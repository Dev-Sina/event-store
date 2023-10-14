using ESDB.Core.Events;

namespace ESDB.Core.Abstraction;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task HandleEventAsync(TEvent @event,
        CancellationToken cancellationToken = default);
}
