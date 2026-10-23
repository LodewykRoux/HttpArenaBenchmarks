using HttpArena.HttpSource;
using Microsoft.AspNetCore.Mvc;

namespace HttpArena.Controller;

[ApiController]
[Route("api")]
public class DataController : ControllerBase
{
    [HttpGet("small")]
    public IActionResult GetSmall() => Ok(SizeTest.GetSmall());

    [HttpGet("medium")]
    public IActionResult GetMedium() => Ok(SizeTest.GetMedium());

    [HttpGet("large")]
    public IActionResult GetLarge() => Ok(SizeTest.GetLarge());
}

public static class ControllerBenchmarks
{
    public static void Run()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        var app = builder.Build();
        app.MapControllers();
        app.Run("http://localhost:5000");
    }
}