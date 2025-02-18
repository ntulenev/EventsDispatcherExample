namespace EventsDispatcher;

/// <summary>
/// Represents an abstraction for a dispatcher used in Domain-Driven Design (DDD).
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Subscribes a handler for a specific type of event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The asynchronous function to handle the event.</param>
    void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler);

    /// <summary>
    /// Unsubscribes a handler for a specific type of event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to unsubscribe from.</typeparam>
    /// <param name="handler">The asynchronous function to unsubscribe.</param>
    void Unsubscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler);

    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="evt">The event to publish.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken);
}
