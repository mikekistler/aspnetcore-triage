using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(x => x.AddSchemaTransformer<Transformer>());

var app = builder.Build();

app.MapOpenApi();
app.MapControllers();

// app.MapGet("/weatherforecast", ResponseBody () => throw new NotImplementedException());

app.Run();

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public ResponseBody Get() => throw new NotImplementedException();
}

public class ResponseBody
{
    public int? id { get; set; }
}

class Transformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema                   schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken               cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(ResponseBody))
        {
            schema.AnyOf =
            [
                new OpenApiSchema
                {
                    Type       = "object",
                    // Properties = new Dictionary<string, OpenApiSchema> { ["optionalProperty"] = new() { Type = "string" } }
                },
            ];
        }

        return Task.CompletedTask;
    }
}