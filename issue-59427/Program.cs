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

// app.MapGet("/weatherforecast", () =>
// {
//     return Enumerable.Empty<WeatherForecast>();
// })
// .Produces<IEnumerable<WeatherForecast>>()
// .WithName("GetWeatherForecasts");

// app.MapPost("/weatherforecast", (WeatherForecast weatherForecast) =>
// {
//     return weatherForecast;
// })
// .Produces<WeatherForecast>()
// .WithName("CreateWeatherForecast");

app.MapPost("/location", () =>
{
    return new LocationDto();
})
.Produces<LocationDto>();

app.Run();

// public class WeatherForecast
// {
//     public LocationDto Location { get; set; }
// }

public class LocationDto
{
    public AddressDto Address { get; set; }
}

public class AddressDto
{
    public LocationDto RelatedLocation { get; set; }
}
