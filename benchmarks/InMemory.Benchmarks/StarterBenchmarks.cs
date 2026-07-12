using Atya.Messaging.InMemory;
using BenchmarkDotNet.Attributes;

namespace Atya.Messaging.InMemory.Benchmarks;

/// <summary>
/// Benchmarks for in-memory publish operations.
/// </summary>
[MemoryDiagnoser]
public class StarterBenchmarks
{
    private InMemoryMessageBus<string> bus = new();

    /// <summary>
    /// Creates a bus for each benchmark iteration.
    /// </summary>
    [IterationSetup]
    public void Setup()
    {
        bus = new InMemoryMessageBus<string>();
    }

    /// <summary>
    /// Publishes a message without consumers.
    /// </summary>
    /// <returns>A task that completes when the message has been stored.</returns>
    [Benchmark]
    public ValueTask PublishWithoutConsumers() => bus.PublishAsync("payload");
}
