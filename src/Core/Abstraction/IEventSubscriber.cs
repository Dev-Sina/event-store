namespace ESDB.Core.Abstraction;

public interface IEventSubscriber
{
    Task SubscribeEventAsync(string streamName, CancellationToken cancellationToken = default);
}
