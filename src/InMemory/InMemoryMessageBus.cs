using Atya.Foundation.Guards;
using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory;

/// <summary>
/// In-memory message publisher and consumer registry.
/// </summary>
/// <typeparam name="TMessage">The message payload type.</typeparam>
public sealed class InMemoryMessageBus<TMessage> :
    IMessagePublisher<TMessage>,
    IMessageConsumerRegistry<TMessage>
{
    private readonly object _gate = new();
    private readonly List<IMessageConsumer<TMessage>> _consumers = [];
    private readonly List<MessageEnvelope<TMessage>> _publishedMessages = [];

    /// <summary>
    /// Gets the messages accepted by this bus.
    /// </summary>
    public IReadOnlyList<MessageEnvelope<TMessage>> PublishedMessages
    {
        get
        {
            lock (_gate)
            {
                return _publishedMessages.ToArray();
            }
        }
    }

    /// <summary>
    /// Gets the number of active consumers.
    /// </summary>
    public int ConsumerCount
    {
        get
        {
            lock (_gate)
            {
                return _consumers.Count;
            }
        }
    }

    /// <inheritdoc />
    public IMessageSubscription Subscribe(IMessageConsumer<TMessage> consumer)
    {
        var validatedConsumer = Guard.AgainstNull(consumer);

        lock (_gate)
        {
            _consumers.Add(validatedConsumer);
        }

        return new InMemoryMessageSubscription<TMessage>(this, validatedConsumer);
    }

    /// <inheritdoc />
    public async ValueTask PublishAsync(
        TMessage message,
        MessagePublishOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var envelope = MessageEnvelope.Create(message, options);
        IMessageConsumer<TMessage>[] snapshot;

        lock (_gate)
        {
            _publishedMessages.Add(envelope);
            snapshot = _consumers.ToArray();
        }

        foreach (var consumer in snapshot)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await consumer.ConsumeAsync(envelope, cancellationToken).ConfigureAwait(false);
        }
    }

    internal bool Remove(IMessageConsumer<TMessage> consumer)
    {
        lock (_gate)
        {
            return _consumers.Remove(consumer);
        }
    }
}
