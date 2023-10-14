namespace ESDB.Core.Models;

public class TransactionModel
{
    public TransactionModel()
    {
    }

    public Guid Guid { get; set; }
    public decimal Amount { get; set; }
}
