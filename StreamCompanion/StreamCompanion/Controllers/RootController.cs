using Microsoft.AspNetCore.Mvc;

using StreamCompanion.Services;

namespace StreamCompanion.Controllers;

[ApiController]
[Route("api")]
public class RootController : ControllerBase
{
    private StreamCompanionService service;

    public RootController(StreamCompanionService streamService)
    {
        service = streamService;
    }

    [HttpGet("description")]
    public object GetDescription()
    {
        return service.GetDescription();
    }
}