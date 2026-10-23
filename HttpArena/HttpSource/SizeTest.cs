namespace HttpArena.HttpSource;

public record SmallPayload(int Id, string Message, DateTime Timestamp);

public record LargePayload(
    int Id, 
    string Title, 
    string Description, 
    string[] Tags
);

public class SizeTest
{
    public static SmallPayload GetSmall() => 
        new(1, "Hello World", DateTime.UtcNow);

    public static IEnumerable<SmallPayload> GetMedium() => 
        Enumerable.Range(1, 50)
            .Select(i => new SmallPayload(i, $"Item {i}", DateTime.UtcNow));

    public static IEnumerable<LargePayload> GetLarge() => 
        Enumerable.Range(1, 2500)
            .Select(i => new LargePayload(
                Id: i,
                Title: $"Title {i}",
                Description: "This is a detailed description string to pad out the payload size for benchmarking.",
                Tags: ["tag1", "tag2", "tag3"]
            ));
}