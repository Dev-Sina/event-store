namespace ESDB.Core.Events;

public class FundsWithdrawedEvent : IEvent
{
    public FundsWithdrawedEvent(Guid guid, decimal amount)
    {
        Guid = guid;
        Amount = amount;
    }

    public Guid Guid { get; private set; }
    public decimal Amount { get; private set; }
}
