# Issue 66304 - OpenAPI omits integer item type when generating int[]

https://github.com/dotnet/aspnetcore/issues/66304

## Problem

When generating a schema for a DTO containing an `int[]`, the property's type is
generated as an array of items with `format: int32` but without `type: integer`.
The same issue also affects non-array integer properties.

## Actual behavior

```json
"values": {
  "type": "array",
  "items": {
    "pattern": "^-?(?:0|[1-9]\\d*)$",
    "format": "int32"
  }
},
"count": {
  "pattern": "^-?(?:0|[1-9]\\d*)$",
  "format": "int32"
}
```

## Expected behavior

```json
"values": {
  "type": "array",
  "items": {
    "type": "integer",
    "format": "int32"
  }
},
"count": {
  "type": "integer",
  "format": "int32"
}
```

## Steps to reproduce

1. Run the app: `dotnet run`
2. Fetch the OpenAPI document: `GET http://localhost:5290/openapi/v1.json`
3. Inspect the schema for `MyDto` — the `values` array items and `count` property
   will have `format: int32` and a regex `pattern` but no `type: integer`.
