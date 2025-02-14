using System.Text.Json.Serialization;

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

app.MapGet("/bug", () => { return "Hello World!"; }).Produces<Dictionary<AuthErrorType, ErrorDetail>>();

app.Run();

[JsonConverter(typeof(JsonStringEnumConverter<AuthErrorType>))]
public enum AuthErrorType
{
    InvalidCredentials,
    InvalidLogin
}

internal class ErrorDetail
{
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}