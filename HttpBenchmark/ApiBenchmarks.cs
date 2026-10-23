using BenchmarkDotNet.Attributes;
using GenHTTP.Modules.ApiBrowsing;
using GenHTTP.Modules.Controllers;
using GenHTTP.Modules.Functional;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.OpenApi;
using GenHTTP.Modules.Practices;
using GenHTTP.Modules.Webservices;
using HttpArena.HttpSource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace HttpBenchmark;

[MemoryDiagnoser]
[MeanColumn, MedianColumn, Q3Column]
public class ApiBenchmarks
{
    private WebApplication? _aspNetApp;
    private GenHTTP.Api.Infrastructure.IServerHost? _genHttpHost;

    private HttpClient _genHttpClient = null!;
    private HttpClient _minimalClient = null!;
    private HttpClient _controllerClient = null!;

    [Params("small", "medium", "large")]
    public string PayloadSize { get; set; } = "small";

    [GlobalSetup]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.WebHost.UseUrls("http://localhost:5001");
        
        _aspNetApp = builder.Build();

        // Minimal API mapping
        _aspNetApp.MapGet("/minimal/small", SizeTest.GetSmall);
        _aspNetApp.MapGet("/minimal/medium", SizeTest.GetMedium);
        _aspNetApp.MapGet("/minimal/large", SizeTest.GetLarge);
        
        // Controller mapping
        _aspNetApp.MapControllers();
        _aspNetApp.Urls.Add("http://localhost:5001");
        
        await _aspNetApp.StartAsync();

        var api = Layout.Create()
            .AddService<GenHttpBenchmarks>("api")
            .AddOpenApi()
            .AddScalar();

        _genHttpHost =  await GenHTTP.Engine.Internal.Host.Create()
            .Handler(api)
            .Port(5002)
            .Logging(NullLoggerFactory.Instance, false)
            .Defaults()
            .StartAsync();
        
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        };
        
        _minimalClient = new HttpClient(handler, false) { BaseAddress = new Uri("http://localhost:5001/minimal/") };
        _controllerClient = new HttpClient(handler, false) { BaseAddress = new Uri("http://localhost:5001/api/") };
        _genHttpClient = new HttpClient(handler, false) { BaseAddress = new Uri("http://localhost:5002/api/") };
        
        await VerifyEndpoint(_minimalClient, PayloadSize);
        await VerifyEndpoint(_controllerClient, PayloadSize);
        await VerifyEndpoint(_genHttpClient, PayloadSize);
    }
    
    private static async Task VerifyEndpoint(HttpClient client, string path)
    {
        using var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();
    }
    
    public class GenHttpBenchmarks
    {
        [ResourceMethod("small")]
        public SmallPayload GetSmall() => SizeTest.GetSmall();

        [ResourceMethod("medium")]
        public IEnumerable<SmallPayload> GetMedium() => SizeTest.GetMedium();

        [ResourceMethod("large")]
        public IEnumerable<LargePayload> GetLarge() => SizeTest.GetLarge();
    }
    
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

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _genHttpClient?.Dispose();
        _minimalClient?.Dispose();
        _controllerClient?.Dispose();

        if (_aspNetApp != null)
        {
            await _aspNetApp.StopAsync();
            await _aspNetApp.DisposeAsync();
        }
    }

    [Benchmark]
    public async Task GenHttp()
    {
        var response = await _genHttpClient.GetAsync(PayloadSize);
        _ = await response.Content.ReadAsByteArrayAsync();
    }

    [Benchmark]
    public async Task MinimalApi()
    {
        var response = await _minimalClient.GetAsync(PayloadSize);
        _ = await response.Content.ReadAsByteArrayAsync();
    }

    [Benchmark]
    public async Task Controller()
    {
        var response = await _controllerClient.GetAsync(PayloadSize);
        _ = await response.Content.ReadAsByteArrayAsync();
    }
}