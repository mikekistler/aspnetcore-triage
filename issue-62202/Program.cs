using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<JsonExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.MapPost("/api/ping", (PingSource source) =>
{
    return TypedResults.Ok(source);
});

app.Run();

public record class PingSource(string Value);

public class JsonExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Only handle JsonException, let other exceptions be handled by other handlers
        if (exception.InnerException is not JsonException jsonException)
        {
            return false;
        }

        Dictionary<string, string[]>? errors = new() { { jsonException.Path ?? string.Empty, new[] { jsonException.Message } } };

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred."
        };

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetailsService.TryWriteAsync(context);
    }
}