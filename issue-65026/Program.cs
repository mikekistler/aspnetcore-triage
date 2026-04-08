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

app.Run();

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
