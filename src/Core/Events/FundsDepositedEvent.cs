namespace ESDB.Core.Events;

public class FundsDepositedEvent : IEvent
{
    public FundsDepositedEvent(Guid guid, decimal amount)
    {
        Guid = guid;
        Amount = amount;
    }

    public Guid Guid { get; private set; }
    public decimal Amount { get; private set; }
}
