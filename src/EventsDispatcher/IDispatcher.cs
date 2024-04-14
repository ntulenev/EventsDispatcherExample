namespace EventsDispatcher;

public interface IDispatcher
{
    public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler);
    public void Unsubscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler);
    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken);
}