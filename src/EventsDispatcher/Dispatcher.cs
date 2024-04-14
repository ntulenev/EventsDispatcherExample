using System.Collections.Immutable;

namespace EventsDispatcher;

public sealed class Dispatcher : IDispatcher
{
    public Dispatcher()
    {
        _handlers = ImmutableDictionary<Type, ImmutableList<Delegate>>.Empty;
    }

    public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
    {
        var eventType = typeof(TEvent);
        var newHandlers = ImmutableList<Delegate>.Empty.Add(handler);

        lock (_lock)
        {
            if (_handlers.TryGetValue(eventType, out var existingHandlers))
            {
                newHandlers = existingHandlers.Add(handler);
            }

            _handlers = _handlers.SetItem(eventType, ImmutableList<Delegate>.Empty.Add(handler));
        }
    }

    public void Unsubscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
    {
        var eventType = typeof(TEvent);

        lock (_lock)
        {
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                var newHandlers = handlers.Remove( handler);
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

    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
    {
        var eventType = typeof(TEvent);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var handlerTasks = handlers.SelectMany(x=>x.GetInvocationList())
                .Cast<Func<TEvent,CancellationToken,Task>>()
                .Select(handler => handler(evt, cancellationToken));
            await Task.WhenAll(handlerTasks);
        }
    }
    
    private ImmutableDictionary<Type, ImmutableList<Delegate>> _handlers;
    private readonly object _lock = new object();
}