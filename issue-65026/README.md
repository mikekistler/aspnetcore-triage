# Issue 65026: Mismatch between OpenAPI spec and enum deserialization

- **Issue**: [dotnet/aspnetcore#65026](https://github.com/dotnet/aspnetcore/issues/65026)
- **Area**: OpenAPI, Minimal APIs, Parameter Binding
- **Status**: Open

## Problem

When `JsonStringEnumConverter` is configured with a naming policy (e.g. `KebabCaseLower`),
the OpenAPI document generator correctly reflects the transformed enum values in the spec
(`the-value`, `my-value`, `your-value`). However, query/path/header parameter binding uses
`Enum.TryParse` which only recognizes the original C# member names (`TheValue`, `MyValue`,
`YourValue`).

This means a client following the OpenAPI spec will send kebab-case values that the server
rejects with 400 Bad Request.

## Repro

```bash
dotnet run --project issue-65026
```

Then test:

| Request | Expected | Actual |
|---------|----------|--------|
| `GET /enum-value?test=my-value` | 200 `"my-value"` | **400 Bad Request** (bug) |
| `GET /enum-value?test=MyValue` | 200 `"my-value"` | 200 `"my-value"` |
| `POST /enum-form` with `test=my-value` | 200 `"my-value"` | **400 Bad Request** (bug) |
| `POST /enum-form` with `test=MyValue` | 200 `"my-value"` | 200 `"my-value"` |

The `.http` file has pre-built requests for both cases.

## Root Cause

There are two independent code paths:

1. **OpenAPI generation** reads the configured `JsonSerializerOptions` (including
   `JsonStringEnumConverter` and its naming policy) to produce enum values in the spec.

2. **Query/path/header parameter binding** in minimal APIs uses `Enum.TryParse` (via
   the `IParsable<T>`/`TryParse` pattern) which knows nothing about JSON serializer
   configuration. JSON serializer options are only applied when binding from the **body**
   (`[FromBody]`).

This is a known architectural gap — see also:
- [#49398](https://github.com/dotnet/aspnetcore/issues/49398) — JSON converter ignored for query enum binding
- [#56907](https://github.com/dotnet/aspnetcore/issues/56907) — Enum ignoring naming policy when binding from route
- [#62338](https://github.com/dotnet/aspnetcore/issues/62338) — `JsonStringEnumMemberName` doesn't work for query params
- [#48346](https://github.com/dotnet/aspnetcore/issues/48346) — Request for ergonomic case-insensitive enum parsing (Backlog)

## Analysis of Potential Solutions

### Option A: Make the OpenAPI generator match the binding behavior

The OpenAPI generator could emit enum values using the C# member names (PascalCase) for
non-body parameters, ignoring the JSON naming policy for query/path/header params. This
ensures the spec matches what the server actually accepts.

- ✅ Simple, spec-accurate today
- ❌ Users lose the ability to have ergonomic kebab/snake-case enum values in their API surface
- ❌ Inconsistent — body and query enums would use different casing in the same spec

### Option B: Make parameter binding honor JSON serializer options

Extend the minimal API binding pipeline so that enum parameters bound from query/path/header
also consult the configured `JsonSerializerOptions`. This could be done by:
- Using `JsonSerializer.Deserialize<T>($"\"{value}\"", options)` as a fallback when
  `Enum.TryParse` fails
- Or building a lookup table from the serializer's enum converter at startup

- ✅ Fixes the root cause — spec and behavior align
- ✅ Consistent with body binding
- ❌ Performance implications of JSON deserialization for simple query params
- ❌ Significant change to the binding pipeline — potential breaking change for apps relying
  on PascalCase query values

### Option C: Support a custom `TryParse` via `IParsable<T>` wrappers

Allow users to define a wrapper type that implements `IParsable<T>` with custom parsing logic
that respects the JSON naming policy. This is already possible today as a workaround but
requires boilerplate for each enum type.

- ✅ No framework changes needed
- ❌ Verbose — requires a wrapper per enum type
- ❌ Doesn't solve the OpenAPI spec mismatch automatically

### Option D: Introduce a parameter binding hook or attribute

Add an attribute (e.g. `[BindWithJsonConverter]`) or a service-level option that tells the
binding pipeline to use the JSON serializer for specific parameter types. This gives users
opt-in control without changing default behavior.

- ✅ Non-breaking, opt-in
- ✅ Flexible — could apply to any type, not just enums
- ❌ Discoverability — users must know to apply the attribute
- ❌ Still a gap between OpenAPI (which auto-applies the policy) and binding (which requires opt-in)

### Recommendation

**Option B** is the most correct long-term fix — the framework should make the OpenAPI spec
and parameter binding consistent by default. Option D could serve as a stepping stone if a
non-breaking, opt-in approach is preferred for the initial fix.
