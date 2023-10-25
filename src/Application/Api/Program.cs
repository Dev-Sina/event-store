using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using ESDB.Core.Models;
using ESDB.Services.Repository;
using ESDB.Services.EventsHandlers;
using ESDB.Services.EventsHandlers.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ESDB.Application.Api;

public static class Program
{
    public static async Task Main(params string[] args)
    {
        string esdbConnectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
        using var host = CreateHostBuilder(args, esdbConnectionString).Build();

        var repository = host.Services.GetRequiredService<IEventStoreRepository>();
        var eventHandlerFactory = host.Services.GetRequiredService<IEventHandlerFactory>();

        string streamName = $"sina-{DateTime.UtcNow}";
        Guid aggregateId = Guid.NewGuid();

        Random rnd = new();

        int numberOfEvents = rnd.Next(3, 60);
        Console.WriteLine("");
        Console.Write("Number of all events : ");
        Console.Write(numberOfEvents);
        Console.WriteLine();
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new AccountCreatedEvent(aggregateId, "Sina Bahmanpour"));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        for (int i = 0; i < numberOfEvents; i++)
        {
            bool isDeposite = new List<bool>(2) { false, true } [rnd.Next(2)];
            decimal amount = rnd.Next(1, 20) * 10;
            IEvent? @event = null;
            if (isDeposite)
            {
                @event = new FundsDepositedEvent(aggregateId, amount);
            }
            else
            {
                @event = new FundsWithdrawedEvent(aggregateId, amount);
            }

            await repository.AddEventToStream(streamName, @event);
            await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.Write("Please press a key to exit...");
        Console.ReadLine();
    }

    public static IHostBuilder CreateHostBuilder(string[] args,
        string esdbConnectionString)
    {
        var builder = Host.CreateDefaultBuilder();

        builder
            .ConfigureServices(services =>
            {
                services.AddScoped<BankAccountModel>();
                services.AddTransient<TransactionModel>();
                services.AddTransient<IEventHandler<AccountCreatedEvent>, AccountCreatedEventHandler>();
                services.AddTransient<IEventHandler<FundsDepositedEvent>, FundsDepositedEventHandler>();
                services.AddTransient<IEventHandler<FundsWithdrawedEvent>, FundsWithdrawedEventHandler>();
                services.AddSingleton<IEventStoreRepository>(serviceProvider => new EventStoreRepository(esdbConnectionString));
                services.AddSingleton<IEventHandlerFactory, EventHandlerFactory>();
            });

        return builder;
    }
}
