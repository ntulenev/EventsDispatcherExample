using System.Collections.Immutable;

namespace EventsDispatcher;

/// <summary>
/// Implements the <see cref="IDispatcher"/> interface,
/// providing functionality for subscribing to, unsubscribing from,
/// and publishing events asynchronously.
/// </summary>
public sealed class Dispatcher : IDispatcher
{
    private ImmutableDictionary<Type, ImmutableList<Delegate>> _handlers;
    private readonly object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="Dispatcher"/> class.
    /// </summary>
    public Dispatcher()
    {
        _handlers = ImmutableDictionary<Type, ImmutableList<Delegate>>.Empty;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is null.</exception>
    public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var eventType = typeof(TEvent);
        var newHandlers = ImmutableList<Delegate>.Empty.Add(handler);

        lock (_lock)
        {
            if (_handlers.TryGetValue(eventType, out var existingHandlers))
            {
                newHandlers = existingHandlers.Add(handler);
            }

            _handlers = _handlers.SetItem(eventType, newHandlers);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is null.</exception>
    public void Unsubscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var eventType = typeof(TEvent);

        lock (_lock)
        {
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                var newHandlers = handlers.Remove(handler);
                if (newHandlers.Count == 0)
                {
                    _handlers = _handlers.Remove(eventType);
                }
                else
                {
                    _handlers = _handlers.SetItem(eventType, newHandlers);
                }
            }
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evt"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when
    /// <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(evt);
        cancellationToken.ThrowIfCancellationRequested();

        var eventType = typeof(TEvent);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var handlerTasks = handlers.SelectMany(x => x.GetInvocationList())
                .Cast<Func<TEvent, CancellationToken, Task>>()
                .Select(handler => handler(evt, cancellationToken));
            await Task.WhenAll(handlerTasks);
        }
    }
}