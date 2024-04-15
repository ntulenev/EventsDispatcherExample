using FluentAssertions;

namespace EventsDispatcher.Tests;

public class DispatcherTests
{
    [Fact(DisplayName = nameof(Dispatcher) + " adds a handler for a specific event type")]
    [Trait("Category", "Unit")]
    public void SubscribeAddsHandlerForEventType()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        Func<TestEvent, CancellationToken, Task> handler = (evt, ct) => Task.CompletedTask;

        // Act
        var ex = Record.Exception(() => dispatcher.Subscribe(handler));

        // Assert
        ex.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " removes a handler for a specific event type")]
    [Trait("Category", "Unit")]
    public void UnsubscribeRemovesHandlerForEventType()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        Func<TestEvent, CancellationToken, Task> handler = (evt, ct) => Task.CompletedTask;
        dispatcher.Subscribe(handler);

        // Act
        var ex = Record.Exception(() => dispatcher.Unsubscribe(handler));

        // Assert
        ex.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " invokes all handlers for a specific event type")]
    [Trait("Category", "Unit")]
    public async Task PublishAsyncInvokesHandlersForEventType()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        var eventRaised = false;
        Func<TestEvent, CancellationToken, Task> handler = (evt, ct) =>
        {
            eventRaised = true;
            return Task.CompletedTask;
        };
        dispatcher.Subscribe(handler);
        var cancellationToken = new CancellationToken();

        // Act
        await dispatcher.PublishAsync(new TestEvent(), cancellationToken);

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " does not throw when no handlers are registered")]
    [Trait("Category", "Unit")]
    public async Task PublishAsyncDoesNotThrowWhenNoHandlersRegistered()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        var cancellationToken = new CancellationToken();

        // Act
        var ex = await Record.ExceptionAsync(async () =>
            await dispatcher.PublishAsync(new TestEvent(), cancellationToken));

        // Assert
        ex.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " throws ArgumentNullException when subscribing with null handler")]
    [Trait("Category", "Unit")]
    public void SubscribeThrowsArgumentNullExceptionWhenHandlerIsNull()
    {
        // Arrange
        var dispatcher = new Dispatcher();

        // Act
        Action act = () => dispatcher.Subscribe<TestEvent>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " throws ArgumentNullException when unsubscribing with null handler")]
    [Trait("Category", "Unit")]
    public void UnsubscribeThrowsArgumentNullExceptionWhenHandlerIsNull()
    {
        // Arrange
        var dispatcher = new Dispatcher();

        // Act
        Action act = () => dispatcher.Unsubscribe<TestEvent>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = nameof(Dispatcher) + " throws ArgumentNullException when publishing with null event")]
    [Trait("Category", "Unit")]
    public async Task PublishAsyncThrowsArgumentNullExceptionWhenEventIsNull()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            dispatcher.PublishAsync<TestEvent>(null!, cancellationToken));
    }

    [Fact(DisplayName = nameof(Dispatcher) + " throws OperationCanceledException when " +
                        "publishing with canceled CancellationToken")]
    [Trait("Category", "Unit")]
    public async Task PublishAsyncThrowsOperationCanceledExceptionWhenCancellationTokenIsCanceled()
    {
        // Arrange
        var dispatcher = new Dispatcher();
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            dispatcher.PublishAsync(new TestEvent(), cancellationToken));
    }
}