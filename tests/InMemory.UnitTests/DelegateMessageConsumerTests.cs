using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory.UnitTests;

public sealed class DelegateMessageConsumerTests
{
    [Fact]
    public void Constructor_With_Null_Delegate_Throws()
    {
        var act = () => new DelegateMessageConsumer<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task ConsumeAsync_Invokes_Delegate()
    {
        MessageEnvelope<string>? received = null;
        var consumer = new DelegateMessageConsumer<string>((envelope, _) =>
        {
            received = envelope;
            return ValueTask.CompletedTask;
        });
        var expected = new MessageEnvelope<string>("message-1", "payload");

        await consumer.ConsumeAsync(expected, TestContext.Current.CancellationToken);

        received.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task ConsumeAsync_With_Canceled_Token_Throws()
    {
        var consumer = new DelegateMessageConsumer<string>((_, _) => ValueTask.CompletedTask);
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        var act = async () => await consumer.ConsumeAsync(
            new MessageEnvelope<string>("message-1", "payload"),
            source.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
