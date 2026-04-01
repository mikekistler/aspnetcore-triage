using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace issue_64858.Controllers;

[ApiController]
[Route("[controller]")]
public class TriageController : ControllerBase
{
    private readonly ILogger<TriageController> _logger;

    public TriageController(ILogger<TriageController> logger)
    {
        _logger = logger;
    }

    // [HttpGet("original")]
    // [ProducesResponseType<IReadOnlyCollection<WeatherForecast>>(StatusCodes.Status200OK, Description = "OK")]
    // [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Bad Request")]
    // [ProducesResponseType(StatusCodes.Status409Conflict, Description = "Conflict")]
    // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Description = "Unprocessable")]
    // public IActionResult GetOriginal()
    // {
    //     if (DateTime.Now.Day == 3)
    //         return Conflict();
    //     if (DateTime.Now.Day == 7)
    //         return BadRequest();
    //     if (DateTime.Now.Day == 5)
    //         return UnprocessableEntity();

    //     return Ok(Array.Empty<WeatherForecast>());
    // }

    [HttpGet("typedresults")]
    public Results<Ok<WeatherForecast[]>, BadRequest<ProblemDetails>, Conflict<ProblemDetails>, UnprocessableEntity<ProblemDetails>> Get()
    {
        if (DateTime.Now.Day == 7)
            return TypedResults.Conflict<ProblemDetails>(new ());
        if (DateTime.Now.Day == 4)
            return TypedResults.BadRequest<ProblemDetails>(new ());
        if (DateTime.Now.Day == 5)
            return TypedResults.UnprocessableEntity<ProblemDetails>(new ());

        return TypedResults.Ok(Array.Empty<WeatherForecast>()); // NO Explicit cast required
    }
}
