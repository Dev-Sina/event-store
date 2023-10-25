namespace ESDB.Core.Models;

public class TransactionModel
{
    public TransactionModel()
    {
    }

    public Guid EventGuid { get; set; }
    public Guid AggregateGuid { get; set; }
    public decimal Amount { get; set; }
}
