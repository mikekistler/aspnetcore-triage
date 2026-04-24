using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serializer to use kebab-case for enums
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.TypeInfoResolver = SerializerContext.Default;
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

// BUG: The OpenAPI spec lists enum values in kebab-case (the-value, my-value, your-value)
// because it honors the JsonStringEnumConverter naming policy.
// However, query parameter binding does NOT use the JSON serializer options,
// so passing ?test=my-value returns 400 Bad Request.
// Only the PascalCase form (?test=MyValue) actually works.
app.MapGet("/enum-value", (TestEnum test) => test).WithName("GetValue");

// Same bug with form fields: OpenAPI spec lists kebab-case enum values,
// but [FromForm] binding uses Enum.TryParse which only recognizes PascalCase.
app.MapPost("/enum-form", ([Microsoft.AspNetCore.Mvc.FromForm] TestEnum test) => test)
    .WithName("PostFormValue");

// WORKAROUND (Option C): Use an IParsable<T> wrapper that deserializes via JSON,
// which honors the configured JsonStringEnumConverter naming policy.
// Accepts kebab-case values like ?test=my-value as documented in the OpenAPI spec.
app.MapGet("/enum-value-workaround", (JsonParsable<TestEnum> test) => test.Value)
    .WithName("GetValueWorkaround");

app.Run();

/// <summary>
/// Generic IParsable wrapper that uses JsonSerializer to parse the value,
/// honoring configured JsonStringEnumConverter naming policies.
/// </summary>
readonly record struct JsonParsable<T>(T Value) : IParsable<JsonParsable<T>>
{
    private static JsonSerializerOptions? _cachedOptions;

    public static JsonParsable<T> Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
            throw new FormatException($"Cannot parse '{s}' as {typeof(T).Name}");
        return result;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out JsonParsable<T> result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        try
        {
            // Resolve the HTTP JSON options from DI on first use
            _cachedOptions ??= ResolveOptions(provider);
            var value = JsonSerializer.Deserialize<T>($"\"{s}\"", _cachedOptions);
            result = new JsonParsable<T>(value!);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    private static JsonSerializerOptions ResolveOptions(IFormatProvider? provider)
    {
        // Minimal APIs don't pass a useful IFormatProvider, so we fall back to
        // a default that matches the typical ConfigureHttpJsonOptions setup.
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
        return options;
    }
}

[JsonSerializable(typeof(TestEnum))]
internal partial class SerializerContext : JsonSerializerContext
{
}

enum TestEnum
{
    TheValue,
    MyValue,
    YourValue,
}
