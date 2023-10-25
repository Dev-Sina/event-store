namespace ESDB.Core.Abstraction;

public interface IEventPublisher : IDisposable, IAsyncDisposable
{
    Task PublishEventAsync(string streamName,
        object @event,
        CancellationToken cancellationToken = default);
}
