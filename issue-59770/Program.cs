using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = SourceGenerationContext.Default;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/stream-param", ([FromBody]Stream stream) =>
{
    using var reader = new StreamReader(stream);
    var body = reader.ReadToEndAsync().Result;
    return TypedResults.Ok(body);
})
.WithName("stream-param");

// [JsonSerializable(typeof(bool?))]
//Here's a recreate for #56021 -- it produces a similar exception
// app.MapGet("/bug", ([Description("Whether to return the GUID in uppercase.")] bool? uppercase) =>
// {
//     return TypedResults.Ok(uppercase.HasValue && uppercase.Value ? "OK" : "Not OK");
// });

app.Run();

[JsonSerializable(typeof(Stream))]
[JsonSerializable(typeof(string))]
internal partial class SourceGenerationContext : JsonSerializerContext {}