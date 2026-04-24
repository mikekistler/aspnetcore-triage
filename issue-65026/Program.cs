using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serializer to use kebab-case for enums
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

// OpenAPI spec lists kebab-case enum values, but [FromQuery] binding uses Enum.TryParse
// which only recognizes PascalCase.
app.MapGet("/enum-query", (TestEnum test) => test);

// Same bug with form fields: OpenAPI spec lists kebab-case enum values,
// but [FromForm] binding uses Enum.TryParse which only recognizes PascalCase.
app.MapPost("/enum-form", ([FromForm] TestEnum test) => test)
    .DisableAntiforgery();

// Enum in path parameter
app.MapGet("/enum-path/{test}", (TestEnum test) => test);

// Enum query parameter with a default value — does the default show kebab-case in the spec?
app.MapGet("/enum-query-default", ([DefaultValue(TestEnum.MyValue)] TestEnum test = TestEnum.MyValue) => test);

// Enum in header
app.MapGet("/enum-header", ([FromHeader(Name = "X-Test-Enum")] TestEnum test) => test);

// Enum with duplicate underlying values — does schema collapse aliases?
app.MapGet("/enum-dupes", (DupeEnum test) => test);

app.Run();

enum TestEnum
{
    TheValue,
    MyValue,
    YourValue,
}

// Enum where B and C share the same underlying value
enum DupeEnum
{
    ValueA = 0,
    ValueB = 1,
    ValueC = 1,
}
