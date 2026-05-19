using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Issue: enum route parameters are parsed case-sensitively,
// even though the rest of the routing stack is case-insensitive.
// e.g. /hello/None/world -> 200 OK
//      /hello/none/world -> 400 Bad Request (unexpected!)

// Route parameter
app.MapGet("hello/{status}/world", (Status status) => Results.Ok(status));

// Query parameter
app.MapGet("/query", ([Microsoft.AspNetCore.Mvc.FromQuery] Status status) => Results.Ok(status));

// Header parameter
app.MapGet("/header", ([Microsoft.AspNetCore.Mvc.FromHeader(Name = "X-Status")] Status status) => Results.Ok(status));

// Form parameter
app.MapPost("/form", ([Microsoft.AspNetCore.Mvc.FromForm] Status status) => Results.Ok(status))
    .DisableAntiforgery();

// JSON body parameter
app.MapPost("/body", (StatusRequest request) => Results.Ok(request))
    .DisableAntiforgery();

app.Run();

record StatusRequest(string Name, Status Status);

[JsonConverter(typeof(JsonStringEnumConverter))]
enum Status
{
    None,
    Active,
    Inactive
}
