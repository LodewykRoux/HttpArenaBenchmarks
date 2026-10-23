using HttpArena.HttpSource;

namespace HttpArena.MinimalApi;

public static class MinimalApiBenchmarks
{
    public static void Run()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        app.MapGet("/api/small", SizeTest.GetSmall);

        app.MapGet("/api/medium", SizeTest.GetMedium);

        app.MapGet("/api/large", SizeTest.GetLarge);

        app.Run("http://localhost:5000");
    }
}