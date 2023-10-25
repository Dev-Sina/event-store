namespace ESDB.Core.Events;

public interface IEvent
{
    Guid Guid { get; }
    Guid AggregateGuid { get; }
}
