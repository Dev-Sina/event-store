using System.Text.Json.Serialization;

namespace ESDB.Core.Events;

public class FundsWithdrawedEvent : IEvent
{
    [JsonConstructor]
    public FundsWithdrawedEvent(Guid guid,
        Guid aggregateGuid,
        decimal amount)
    {
        Guid = guid;
        AggregateGuid = aggregateGuid;
        Amount = amount;
    }

    public FundsWithdrawedEvent(Guid aggregateGuid,
        decimal amount)
    {
        Guid = Guid.NewGuid();
        AggregateGuid = aggregateGuid;
        Amount = amount;
    }

    public Guid Guid { get; }
    public Guid AggregateGuid { get; }
    public decimal Amount { get; }
}
