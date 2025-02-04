using Microsoft.AspNetCore.Mvc;

namespace issue_11544_mvc.Controllers;

[ApiController]
public class Issue11544Controller : ControllerBase
{
    private readonly ILogger<Issue11544Controller> _logger;

    public Issue11544Controller(ILogger<Issue11544Controller> logger)
    {
        _logger = logger;
    }

    [HttpGet("test/{p}")]
    public IActionResult Get(string p, string q)
    {
        // return a object with the parameters and the path
        return Ok(new { p, q, Request.Path });
    }
}
