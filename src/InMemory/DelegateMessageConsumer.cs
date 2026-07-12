using Atya.Foundation.Guards;
using Atya.Messaging.Abstractions;

namespace Atya.Messaging.InMemory;

/// <summary>
/// Adapts a delegate to <see cref="IMessageConsumer{TMessage}"/>.
/// </summary>
/// <typeparam name="TMessage">The message payload type.</typeparam>
public sealed class DelegateMessageConsumer<TMessage> : IMessageConsumer<TMessage>
{
    private readonly Func<MessageEnvelope<TMessage>, CancellationToken, ValueTask> _consume;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateMessageConsumer{TMessage}"/> class.
    /// </summary>
    /// <param name="consume">The delegate invoked for each delivered message.</param>
    public DelegateMessageConsumer(
        Func<MessageEnvelope<TMessage>, CancellationToken, ValueTask> consume)
    {
        _consume = Guard.AgainstNull(consume);
    }

    /// <inheritdoc />
    public ValueTask ConsumeAsync(
        MessageEnvelope<TMessage> envelope,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _consume(Guard.AgainstNull(envelope), cancellationToken);
    }
}
