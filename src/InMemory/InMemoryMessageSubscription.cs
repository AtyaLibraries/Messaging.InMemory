using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory;

/// <summary>
/// Removes an in-memory message consumer subscription.
/// </summary>
/// <typeparam name="TMessage">The message payload type.</typeparam>
public sealed class InMemoryMessageSubscription<TMessage> : IMessageSubscription
{
    private readonly InMemoryMessageBus<TMessage> _bus;
    private readonly IMessageConsumer<TMessage> _consumer;
    private int _unsubscribed;

    internal InMemoryMessageSubscription(
        InMemoryMessageBus<TMessage> bus,
        IMessageConsumer<TMessage> consumer)
    {
        _bus = bus;
        _consumer = consumer;
    }

    /// <inheritdoc />
    public ValueTask UnsubscribeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Interlocked.Exchange(ref _unsubscribed, 1) == 0)
        {
            _bus.Remove(_consumer);
        }

        return ValueTask.CompletedTask;
    }
}
