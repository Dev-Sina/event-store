using ESDB.Core.Events;

namespace ESDB.Core.Models;

public class BankAccountModel
{
    public BankAccountModel()
    {
    }

    public Guid AggregateGuid { get; set; }
    public string Name { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

    public void CalculateCurrentBalance()
    {
        CurrentBalance = Transactions
            .DistinctBy(x => x.EventGuid)
            .Select(x => x.Amount)
            .Sum();
    }

    public void Apply(AccountCreatedEvent @event)
    {
        AggregateGuid = @event.AggregateGuid;
        Name = @event.Name;

        CalculateCurrentBalance();
    }

    public void Apply(FundsDepositedEvent @event)
    {
        if (Transactions.Any(x => x.EventGuid == @event.Guid))
        {
            return;
        }

        var newTransaction = new TransactionModel
        {
            EventGuid = @event.Guid,
            AggregateGuid = @event.AggregateGuid,
            Amount = @event.Amount
        };

        Transactions.Add(newTransaction);
        CalculateCurrentBalance();
    }

    public void Apply(FundsWithdrawedEvent @event)
    {
        if (Transactions.Any(x => x.EventGuid == @event.Guid))
        {
            return;
        }

        var newTransaction = new TransactionModel
        {
            EventGuid = @event.Guid,
            AggregateGuid = @event.AggregateGuid,
            Amount = @event.Amount * -1
        };

        Transactions.Add(newTransaction);
        CalculateCurrentBalance();
    }
}
