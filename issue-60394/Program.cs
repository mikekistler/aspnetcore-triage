using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var api = app.MapGroup("api/talks");

// The ProblemHttpResult is not present in the OpenAPI document, as explained in https://github.com/dotnet/aspnetcore/issues/58719
// To ensure that it is added, we can use a combination of TypedResults and extension methods (which is not ideal).
// We could also fall back to using Results and use extension methods for the OpenAPI metadata, but we'd lose compile-time safety..
api.MapGet("/{id:int:min(1)}", GetTalk)
    .WithName("Talks_GetTalk");
    // .ProducesProblem(StatusCodes.Status404NotFound);

app.Run();

static Results<Ok<TalkModel>, NotFound> GetTalk(int id)
{
     var SampleTalks = new
    {
        Talks = new[]
        {
            new TalkModel(1, "Talk 1", "Speaker 1", DateTime.UtcNow),
            new TalkModel(2, "Talk 2", "Speaker 2", DateTime.UtcNow),
            new TalkModel(3, "Talk 3", "Speaker 3", DateTime.UtcNow),
        }
    };
    // This still excludes the traceid from the ProblemDetails instance for brevity purposes
    var talk = SampleTalks.Talks.FirstOrDefault(x => x.Id == id);
    return talk == null ?
        TypedResults.NotFound() :
        TypedResults.Ok(talk);
}

public record TalkModel(int Id, string Title, string Speaker, DateTime StartTime);
