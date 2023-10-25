using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using ESDB.Core.Models;

namespace ESDB.Services.EventsHandlers.EventHandlers;

public class AccountCreatedEventHandler : IEventHandler<AccountCreatedEvent>
{
    private readonly BankAccountModel _bankAccountModel;

    public AccountCreatedEventHandler(BankAccountModel bankAccountModel)
    {
        _bankAccountModel = bankAccountModel;
    }

    public async Task HandleEventAsync(AccountCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        _bankAccountModel.Apply(@event);

        Console.Write("-- Account Created | Balance: ");
        Console.WriteLine(_bankAccountModel.CurrentBalance);

        await Task.CompletedTask;
        return;
    }
}
