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

app.MapGet("/hello", () => "Hello World!");

app.MapGet("/goodbye", async context =>
{
    await context.Response.WriteAsJsonAsync(new { Message = "Goodbye!" });
}).WithDescription("Goodbye");

app.MapGet("/howdy", async (HttpRequest request) =>
{
    await request.HttpContext.Response.WriteAsJsonAsync(new { Message = "Howdy y'all!" });
}).WithDescription("Texan greeting");

app.Run();
