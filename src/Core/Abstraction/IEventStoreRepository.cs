using ESDB.Core.Events;
using EventStore.Client;

namespace ESDB.Core.Abstraction;

public interface IEventStoreRepository : IDisposable, IAsyncDisposable
{
    Task AddEventToStream(string streamName,
        IEvent @event,
        CancellationToken cancellationToken = default);

    Task AddEventToStream(string streamName,
        IEvent @event,
        Action<EventStoreClientOperationOptions>? configureOperationOptions,
        TimeSpan? deadline,
        UserCredentials? userCredentials,
        CancellationToken cancellationToken = default);

    Task AddEventToStream(string streamName,
        IEvent @event,
        StreamState expectedState,
        Action<EventStoreClientOperationOptions>? configureOperationOptions,
        TimeSpan? deadline,
        UserCredentials? userCredentials,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEvent>> GetStreamEventsAsync<TEvent>(string streamName,
        CancellationToken cancellationToken = default) where TEvent : IEvent;

    Task<IEnumerable<EventRecord>> GetStreamEventsAsync(string streamName,
        CancellationToken cancellationToken = default);
}
