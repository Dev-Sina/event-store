using System.Text;
using ESDB.Core.Abstraction;
using EventStore.Client;
using Newtonsoft.Json;

namespace ESDB.Services.Publisher;

public class EventPublisher : IEventPublisher
{
    private readonly EventStoreClient _client;

    public EventPublisher()
    {
        string connectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";

        var settings = EventStoreClientSettings.Create($"{connectionString}");
        _client = new EventStoreClient(settings);
    }

    public async Task PublishEventAsync(string streamName,
        object @event,
        CancellationToken cancellationToken = default)
    {
        // Specify variables values
        string eventType = @event.GetType().Name;
        var eventData = @event;

        // Convert the event data to a JSON string.
        var eventDataJson = JsonConvert
            .SerializeObject(eventData,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                });
        var eventDataBytes = Encoding.UTF8.GetBytes(eventDataJson);

        // Create an event data object.
        var eventStoreDataType = new EventData(Uuid.NewUuid(),
            eventType,
            eventDataBytes);

        // Publish the event to the specified stream.
        await _client.AppendToStreamAsync(streamName,
            StreamState.Any,
            new List<EventData>(1) { eventStoreDataType },
            null);
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
