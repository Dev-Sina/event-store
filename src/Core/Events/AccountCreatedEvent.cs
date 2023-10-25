using System.Text.Json.Serialization;

namespace ESDB.Core.Events;

public class AccountCreatedEvent : IEvent
{
    [JsonConstructor]
    public AccountCreatedEvent(Guid guid,
        Guid aggregateGuid,
        string name)
    {
        Guid = guid;
        AggregateGuid = aggregateGuid;
        Name = name;
    }

    public AccountCreatedEvent(Guid aggregateGuid,
        string name)
    {
        Guid = Guid.NewGuid();
        AggregateGuid = aggregateGuid;
        Name = name;
    }

    public Guid Guid { get; }
    public Guid AggregateGuid { get; private set; }
    public string Name { get; }
}
