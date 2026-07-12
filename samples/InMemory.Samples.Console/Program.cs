using Atya.Messaging.Abstractions;
using Atya.Messaging.InMemory;

namespace Atya.Messaging.InMemory.Samples.ConsoleApp;

/// <summary>
/// Runs the sample console application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Demonstrates publishing through the in-memory bus.
    /// </summary>
    /// <returns>A task that completes when the sample message has been published.</returns>
    public static async Task Main()
    {
        var bus = new InMemoryMessageBus<string>();
        bus.Subscribe(new DelegateMessageConsumer<string>((envelope, _) =>
        {
            Console.WriteLine($"Consumed {envelope.MessageId}: {envelope.Message}");
            return ValueTask.CompletedTask;
        }));

        await bus.PublishAsync(
            "customer.created",
            new MessagePublishOptions(messageId: "message-1", correlationId: "correlation-1"));
    }
}
