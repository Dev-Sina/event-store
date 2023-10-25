using ESDB.Core.Events;
using EventStore.Client;

namespace ESDB.Core.Abstraction;

public interface IEventHandlerFactory
{
    Task HandleEventAsync(IEvent @event,
        CancellationToken cancellationToken = default);
    
    Task HandleEventsAsync(IEnumerable<IEvent> events,
        CancellationToken cancellationToken = default);

    Task HandleEventRecordAsync(EventRecord eventRecord,
        CancellationToken cancellationToken = default);

    Task HandleEventRecordsAsync(IEnumerable<EventRecord> eventRecords,
        CancellationToken cancellationToken = default);

    Task HandleAllStreamEventRecordsAsync(string streamName,
        CancellationToken cancellationToken = default);
}
