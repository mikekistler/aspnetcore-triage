using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/bug", ([FromBody]Stream body) =>
{
    using var reader = new StreamReader(body);
    var json = reader.ReadToEndAsync().Result;
    Console.WriteLine(json);
    return TypedResults.Ok();
})
.WithName("bug");

app.Run();
