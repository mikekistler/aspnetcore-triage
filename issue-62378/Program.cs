using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

app.MapGet("/Pet", () => Enumerable.Empty<Pet>).WithName("Pet").Produces<Pet[]>();
app.MapGet("/Shape", () => Enumerable.Empty<Shape>).WithName("Shape").Produces<Shape[]>();

app.Run();

[JsonPolymorphic]
[JsonDerivedType(typeof(Cat), "cat")]
[JsonDerivedType(typeof(Dog), "dog")]
public record Pet(string Name);
public record Dog(string Name, string? Breed) : Pet(Name);
public record Cat(string Name, int? Lives) : Pet(Name);

[JsonPolymorphic]
[JsonDerivedType(typeof(Rectangle), "Rectangle")]
[JsonDerivedType(typeof(Circle), "Circle")]
public record Shape();
public record Rectangle(int Width, int Height) : Shape();
public record Circle(int Radius) : Shape();