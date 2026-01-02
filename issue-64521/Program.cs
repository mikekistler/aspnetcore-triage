using System.Runtime.CompilerServices;
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

app.MapGet("/weatherforecast/{routeParam}", GetForecast);

app.Run();

static partial class Program {
    /// <summary>
    /// An endpoint that returns a weather forecast.
    /// </summary>
    /// <param name="routeParam">A route parameter value.</param>
    /// <param name="headerParam">A custom header value.</param>
    /// <param name="queryParam">A query parameter value.</param>
    public static WeatherForecast[] GetForecast(
        [FromRoute] string routeParam,
        [FromHeader(Name = "X-Custom-Header")] string? headerParam = null,
        [FromQuery] string? queryParam = null
    )
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
