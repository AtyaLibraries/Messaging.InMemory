using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory.UnitTests;

public sealed class InMemoryMessageBusTests
{
    [Fact]
    public async Task SubscribeAsync_With_Null_Consumer_Throws()
    {
        var bus = new InMemoryMessageBus<string>();

        var act = async () => await bus.SubscribeAsync(
            null!,
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SubscribeAsync_Adds_Consumer()
    {
        var bus = new InMemoryMessageBus<string>();

        await bus.SubscribeAsync(
            new DelegateMessageConsumer<string>((_, _) => ValueTask.CompletedTask),
            TestContext.Current.CancellationToken);

        bus.ConsumerCount.Should().Be(1);
    }

    [Fact]
    public async Task SubscribeAsync_With_Canceled_Token_Throws_Without_Adding_Consumer()
    {
        var bus = new InMemoryMessageBus<string>();
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        var act = async () => await bus.SubscribeAsync(
            new DelegateMessageConsumer<string>((_, _) => ValueTask.CompletedTask),
            source.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        bus.ConsumerCount.Should().Be(0);
    }

    [Fact]
    public async Task PublishAsync_Stores_And_Delivers_Message()
    {
        var bus = new InMemoryMessageBus<string>();
        MessageEnvelope<string>? received = null;
        await bus.SubscribeAsync(
            new DelegateMessageConsumer<string>((envelope, _) =>
            {
                received = envelope;
                return ValueTask.CompletedTask;
            }),
            TestContext.Current.CancellationToken);

        await bus.PublishAsync(
            "payload",
            new MessagePublishOptions("message-1", "correlation-1"),
            TestContext.Current.CancellationToken);

        bus.PublishedMessages.Should().ContainSingle();
        received.Should().BeSameAs(bus.PublishedMessages[0]);
        received!.MessageId.Should().Be("message-1");
        received.Message.Should().Be("payload");
        received.CorrelationId.Should().Be("correlation-1");
    }

    [Fact]
    public async Task PublishAsync_With_No_Consumers_Still_Stores_Message()
    {
        var bus = new InMemoryMessageBus<string>();

        await bus.PublishAsync("payload", cancellationToken: TestContext.Current.CancellationToken);

        bus.PublishedMessages.Should().ContainSingle();
        bus.PublishedMessages[0].Message.Should().Be("payload");
    }

    [Fact]
    public async Task PublishAsync_With_Canceled_Token_Throws_Without_Storing()
    {
        var bus = new InMemoryMessageBus<string>();
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        var act = async () => await bus.PublishAsync("payload", cancellationToken: source.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        bus.PublishedMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task PublishAsync_When_Consumer_Cancels_Propagates_Cancellation()
    {
        var bus = new InMemoryMessageBus<string>();
        await bus.SubscribeAsync(
            new DelegateMessageConsumer<string>((_, token) =>
            {
                token.ThrowIfCancellationRequested();
                return ValueTask.CompletedTask;
            }),
            TestContext.Current.CancellationToken);
        using var source = new CancellationTokenSource();
        source.Cancel();

        var act = async () => await bus.PublishAsync("payload", cancellationToken: source.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task UnsubscribeAsync_Removes_Consumer()
    {
        var bus = new InMemoryMessageBus<string>();
        var subscription = await bus.SubscribeAsync(
            new DelegateMessageConsumer<string>((_, _) => ValueTask.CompletedTask),
            TestContext.Current.CancellationToken);

        await subscription.UnsubscribeAsync(TestContext.Current.CancellationToken);
        await subscription.UnsubscribeAsync(TestContext.Current.CancellationToken);

        bus.ConsumerCount.Should().Be(0);
    }
}
