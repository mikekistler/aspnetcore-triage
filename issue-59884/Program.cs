var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer<ShowMetadataTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/has-metadata", (HttpRequest request) =>
{
    var result = new Dictionary<string, string>();
    var metadata = request.HttpContext.GetEndpoint()?.Metadata;
    if (metadata != null)
    {
        foreach (var data in metadata)
        {
            result.Add(data.GetType().ToString(), data?.ToString() ?? "NA");
        }
    }
    return TypedResults.Ok(result);
})
.WithName("Metadata")
.WithDescription("Get metadata for the current request.")
.WithSummary("Get metadata")
.WithTags("Metadata")
.Accepts<string>("text/plain");

app.Run();
