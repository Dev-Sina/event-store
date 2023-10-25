namespace ESDB.Core.Abstraction;

public interface IEventPublisher : IDisposable, IAsyncDisposable
{
    Task<string> PublishEventAsync(string streamName,
        object @event,
        CancellationToken cancellationToken = default);
}
