using System.Security.Authentication;
using GenHTTP.Modules.ApiBrowsing;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.OpenApi;
using GenHTTP.Modules.Practices;
using GenHTTP.Modules.Security;
using GenHTTP.Modules.Webservices;
using HttpArena.HttpSource;
using Microsoft.Extensions.Logging.Abstractions;


// using GenHTTP.Engine.Ioxide;
//
// using ioxide;

namespace HttpArena.GenHttp;

public class GenHttpBenchmarks
{
    [ResourceMethod("small")]
    public SmallPayload GetSmall() => SizeTest.GetSmall();

    [ResourceMethod("medium")]
    public IEnumerable<SmallPayload> GetMedium() => SizeTest.GetMedium();

    [ResourceMethod("large")]
    public IEnumerable<LargePayload> GetLarge() => SizeTest.GetLarge();
}

public static class InitializeGenHttp
{
    public static async Task Run()
    {
        // var reactors = int.TryParse(Environment.GetEnvironmentVariable("IOXIDE_REACTORS"), out var rc) ? rc : Environment.ProcessorCount;
        //
        // var kernelTls = true;
        // var tlsFloor = kernelTls ? SslProtocols.Tls13 : SslProtocols.Tls12 | SslProtocols.Tls13;
        //
        // var options = new EngineOptions
        // {
        //     Reactor = new ReactorOptions
        //     {
        //         ReactorCount = reactors,
        //
        //         // RecvSlots and RecvBufferSize are left at the engine's defaults on purpose. A parked
        //         // request keeps its recv buffer - the engine parses headers zero-copy out of it - so the
        //         // slot count is really a ceiling on requests in flight, not just a buffer count. This entry
        //         // used to pin it at 256, carried over from the old BufferRingEntries setting, and the async
        //         // profile paid for it: holding 32000 connections on a 10ms wait, the server flat-lined at
        //         // slots x reactors in flight however many connections were offered, stretching the wait
        //         // rather than serving more. Measured at 12000 connections, 16 reactors:
        //         //
        //         //     256 slots    385,584 rps   wait stretched to 30.8ms
        //         //    4096 slots    934,065 rps   wait held at 10.1ms
        //     },
        //     Tcp = new TcpTransportOptions
        //     {
        //         WriteSlabSize = new TcpTransportOptions().WriteSlabSize,
        //
        //         // Kernel TLS on the way out, which is what this entry measured before the engine made it
        //         // opt-in: ioxide.tls used to default it on, so json-tls has always been kTLS TX with
        //         // OpenSSL still decrypting inbound. It needs the Linux tls module - without it every
        //         // handshake on a secure port fails - so IOXIDE_KTLS=0 turns it off.
        //         //
        //         //     cat /proc/sys/net/ipv4/tcp_available_ulp   # wanted: tls
        //         TxKernelTls = kernelTls,
        //     },
        // };
        
        var api = Layout.Create()
            .AddService<GenHttpBenchmarks>("api")
            .Add(CorsPolicy.Permissive())
            .AddOpenApi()
            .AddScalar();

        var host = await GenHTTP.Engine.Internal.Host.Create()
            .Handler(api)
            .Logging(NullLoggerFactory.Instance, false)
            .Defaults()
            .Port(5000)
            .StartAsync();

        await host.RunAsync();
    }
}