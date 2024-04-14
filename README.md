# EventsDispatcher example

## Overview

This repository contains a C# implementation of an event dispatcher designed to facilitate the handling of events in applications following the Domain-Driven Design (DDD) approach. The event dispatcher supports asynchronous execution of event handlers and ensures thread safety for subscribe, unsubscribe, and publish operations.

## Features

- **Asynchronous Execution**: Event handlers are executed asynchronously, allowing for non-blocking execution of event handling logic.
- **Thread Safety**: The event dispatcher guarantees thread safety for subscribe, unsubscribe, and publish operations, making it suitable for multi-threaded environments.
- **Flexible Subscription**: Multiple event handlers can be subscribed to the same event type, providing flexibility in handling various types of events.

## Usage

1. **Subscribe to Events**: Use the `Subscribe` method to subscribe event handlers to specific event types:

    ```csharp
    var dispatcher = new EventsDispatcher();
    dispatcher.Subscribe<MyEventType>(HandleMyEvent);
    ```

2. **Publish Events**: Use the `Publish` method to publish events:

    ```csharp
    await dispatcher.PublishAsync(new MyEventType());
    ```

3. **Unsubscribe from Events**: Use the `Unsubscribe` method to unsubscribe event handlers:

    ```csharp
    await dispatcher.Unsubscribe<MyEventType>(HandleMyEvent);
    ```

## Example

```csharp
var dispatcher = new EventsDispatcher();

// Subscribe event handler
dispatcher.Subscribe<MyEvent>(HandleMyEvent);

// Publish event
await dispatcher.PublishAsync(new MyEvent("Hello, world!"));

// Unsubscribe event handler
dispatcher.Unsubscribe<MyEvent>(HandleMyEvent);


static async Task HandleMyEvent(MyEvent evt, CancellationToken cancellationToken)
{
    Console.WriteLine(evt.Message);
    await Task.Delay(1000); // Simulate asynchronous work
}

public class MyEvent
{
    public string Message { get; }

    public MyEvent(string message)
    {
        Message = message;
    }
}
```
