using ESDB.Core.Events;

namespace ESDB.Core.Models;

public class BankAccountModel
{
    public BankAccountModel()
    {
    }

    public Guid Guid { get; set; }
    public string Name { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

    public void Apply(AccountCreatedEvent @event)
    {
        Guid = @event.Guid;
        Name = @event.Name;
        CurrentBalance = CurrentBalance;
    }

    public void Apply(FundsDepositedEvent @event)
    {
        var newTransaction = new TransactionModel
        {
            Guid = @event.Guid,
            Amount = @event.Amount
        };

        Transactions.Add(newTransaction);
        CurrentBalance = CurrentBalance + @event.Amount;
    }

    public void Apply(FundsWithdrawedEvent @event)
    {
        var newTransaction = new TransactionModel
        {
            Guid = @event.Guid,
            Amount = @event.Amount
        };

        Transactions.Add(newTransaction);
        CurrentBalance = CurrentBalance - @event.Amount;
    }
}
