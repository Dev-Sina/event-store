using System.Text;
using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using EventStore.Client;

namespace ESDB.Services.Repository;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly EventStoreClient _client;

    public EventStoreRepository(string connectionString)
    {
        //string connectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
        var settings = EventStoreClientSettings.Create($"{connectionString}");
        _client = new EventStoreClient(settings);
    }

    public async Task AddEventToStream(string streamName,
        IEvent @event,
        CancellationToken cancellationToken = default)
    {
        await AddEventToStream(streamName,
            @event,
            null,
            null,
            null,
            cancellationToken);
    }

    public async Task AddEventToStream(string streamName,
        IEvent @event,
        Action<EventStoreClientOperationOptions>? configureOperationOptions,
        TimeSpan? deadline,
        UserCredentials? userCredentials,
        CancellationToken cancellationToken = default)
    {
        await AddEventToStream(streamName,
            @event,
            StreamState.Any,
            configureOperationOptions,
            deadline,
            userCredentials,
            cancellationToken);
    }

    public async Task AddEventToStream(string streamName,
        IEvent @event,
        StreamState expectedState,
        Action<EventStoreClientOperationOptions>? configureOperationOptions,
        TimeSpan? deadline,
        UserCredentials? userCredentials,
        CancellationToken cancellationToken = default)
    {
        // Specify variables values
        string eventType = @event.GetType().Name;
        var eventData = @event;

        // Convert the event data to a JSON string.
        var eventDataJson = Newtonsoft.Json.JsonConvert
            .SerializeObject(eventData,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
                });
        var eventDataBytes = Encoding.UTF8.GetBytes(eventDataJson);

        // Create an event data object.
        var eventStoreDataType = new EventData(Uuid.NewUuid(),
            eventType,
            eventDataBytes);

        // Publish the event to the specified stream.
        try
        {
            await _client
                .AppendToStreamAsync(streamName: streamName,
                    expectedState: expectedState,
                    eventData: new EventData[] { eventStoreDataType },
                    configureOperationOptions: configureOperationOptions,
                    deadline: deadline,
                    userCredentials: userCredentials,
                    cancellationToken: cancellationToken);
        }
        catch (WrongExpectedVersionException ex)
        {
            // Handle concurrency conflicts
            Console.WriteLine($"Concurrency conflict: {ex.Message}");
        }
    }

    public async Task<IEnumerable<TEvent>> GetStreamEventsAsync<TEvent>(string streamName,
        CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var result = _client
            .ReadStreamAsync(Direction.Forwards,
                streamName,
                StreamPosition.Start,
                cancellationToken: default);
        if (result is null)
        {
            return Enumerable.Empty<TEvent>();
        }

        List<ResolvedEvent> resolvedEvents = await result.ToListAsync(cancellationToken);
        if (resolvedEvents is null ||
            !resolvedEvents.Any())
        {
            return Enumerable.Empty<TEvent>();
        }

        var events = resolvedEvents
            .Select(x => x.Event)
            .Where(y =>
                y is not null &&
                y.EventType.Equals(typeof(TEvent).Name, StringComparison.OrdinalIgnoreCase))
            .OrderBy(y => y.Created)
            .Select(y =>
            {
                var eventByteArrayData = y.Data;
                var eventSerializedData = Encoding.UTF8.GetString(eventByteArrayData.ToArray());
                TEvent? eventObject = System.Text.Json.JsonSerializer.Deserialize<TEvent?>(eventSerializedData);
                return eventObject;
            })
            .Where(z => z is not null) ?? Enumerable.Empty<TEvent>();

        return events!;
    }

    public async Task<IEnumerable<EventRecord>> GetStreamEventsAsync(string streamName,
        CancellationToken cancellationToken = default)
    {
        var result = _client
            .ReadStreamAsync(Direction.Forwards,
                streamName,
                StreamPosition.Start,
                cancellationToken: default);
        if (result is null)
        {
            return Enumerable.Empty<EventRecord>();
        }

        List<ResolvedEvent> resolvedEvents = await result.ToListAsync(cancellationToken);
        if (resolvedEvents is null ||
            !resolvedEvents.Any())
        {
            return Enumerable.Empty<EventRecord>();
        }

        var events = resolvedEvents
            .Select(x => x.Event)
            .OrderBy(x => x.Created)
            .Where(x => x is not null) ?? Enumerable.Empty<EventRecord>();

        return events;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
