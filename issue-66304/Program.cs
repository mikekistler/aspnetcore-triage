var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/items", (MyDto dto) => Results.Ok(dto))
    .WithName("PostItems");

app.MapGet("/items", () => new MyDto { Name = "Test", Values = [1, 2, 3], Count = 42 })
    .WithName("GetItems");

app.Run();

class MyDto
{
    public string Name { get; set; } = string.Empty;
    public int[] Values { get; set; } = [];
    public int Count { get; set; }
    public int? NullableCount { get; set; }
}
