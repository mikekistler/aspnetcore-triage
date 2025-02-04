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

app.MapGet("/test/{p}", (HttpRequest req, string p, string q) =>
{
    // return a object with the parameters and the path
    return TypedResults.Ok(new { p, q, req.Path });
});

app.Run();
