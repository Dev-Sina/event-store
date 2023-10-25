using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using ESDB.Core.Models;

namespace ESDB.Services.EventsHandlers.EventHandlers;

public class FundsDepositedEventHandler : IEventHandler<FundsDepositedEvent>
{
    private readonly BankAccountModel _bankAccountModel;

    public FundsDepositedEventHandler(BankAccountModel bankAccountModel)
    {
        _bankAccountModel = bankAccountModel;
    }

    public async Task HandleEventAsync(FundsDepositedEvent @event,
        CancellationToken cancellationToken = default)
    {
        _bankAccountModel.Apply(@event);

        Console.Write("-- Funds Deposited | Amount: ");
        Console.Write(@event.Amount);
        Console.Write(" | Balance: ");
        Console.WriteLine(_bankAccountModel.CurrentBalance);

        await Task.CompletedTask;
        return;
    }
}
