using HttpArena.Controller;
using HttpArena.GenHttp;
using HttpArena.HttpSource;
using HttpArena.MinimalApi;

HttpSource source;

if (args.Length > 0 && Enum.TryParse<HttpSource>(args[0], true, out var parsedSource))
{
    source = parsedSource;
}
else
{
    // Fallback prompt if no arguments are passed
    Console.WriteLine("No argument provided. Please select a benchmark implementation to run:");
    Console.WriteLine("  [0] GenHttp");
    Console.WriteLine("  [1] MinimalApi");
    Console.WriteLine("  [2] Controller");
    Console.Write("\nEnter choice (name or number): ");

    var input = Console.ReadLine()?.Trim();

    if (int.TryParse(input, out var intChoice) && Enum.IsDefined(typeof(HttpSource), intChoice))
    {
        source = (HttpSource)intChoice;
    }
    else if (Enum.TryParse<HttpSource>(input, true, out var stringParsed))
    {
        source = stringParsed;
    }
    else
    {
        Console.WriteLine("Invalid selection. Defaulting to Controller.");
        source = HttpSource.Controller;
    }
}

switch (source)
{
    case HttpSource.GenHttp:
        await InitializeGenHttp.Run();
        break;
    case HttpSource.MinimalApi:
        MinimalApiBenchmarks.Run();
        break;
    case HttpSource.Controller:
        ControllerBenchmarks.Run();
        break;
}


