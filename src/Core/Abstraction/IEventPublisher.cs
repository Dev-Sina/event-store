namespace ESDB.Core.Abstraction;

public interface IEventPublisher : IDisposable, IAsyncDisposable
{
    Task<string> PublishEventAsync(object @event,
        CancellationToken cancellationToken = default);
}
