using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using ESDB.Core.Models;

namespace ESDB.Services.Subscription.EventHandlers;

public class FundsWithdrawedEventHandler : IEventHandler<FundsWithdrawedEvent>
{
    private readonly BankAccountModel _bankAccountModel;

    public FundsWithdrawedEventHandler(BankAccountModel bankAccountModel)
    {
        _bankAccountModel = bankAccountModel;
    }

    public async Task HandleEventAsync(FundsWithdrawedEvent @event,
        CancellationToken cancellationToken = default)
    {
        _bankAccountModel.Apply(@event);

        Console.Write("-- Funds Withdrawed | Balance: ");
        Console.Write(@event.Amount);
        Console.Write(" | Balance: ");
        Console.WriteLine(_bankAccountModel.CurrentBalance);

        await Task.CompletedTask;
        return;
    }
}
