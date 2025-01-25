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

app.MapGet("/turtle-events", () => new GetTurtleEventsResponse([
    new TurtleEvent.Added(DateTime.Now, 1),
    new TurtleEvent.Added(DateTime.Now, 2),
    new TurtleEvent.Added(DateTime.Now, 3)
])).WithName("GetTurtleEvents");

app.MapGet("/bear-events", () => new GetBearEventsResponse([
    new BearEvent.Added(DateTime.Now, 1),
    new BearEvent.Added(DateTime.Now, 2),
    new BearEvent.Added(DateTime.Now, 3)
])).WithName("GetBearEvents");

app.Run();

public record GetTurtleEventsResponse(List<TurtleEvent> Events);

public record GetBearEventsResponse(List<BearEvent> Events);
