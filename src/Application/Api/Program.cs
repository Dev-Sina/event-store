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

        await repository.AddEventToStream(streamName, new AccountCreatedEvent(aggregateId, "Sina Bahmanpour"));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new FundsDepositedEvent(aggregateId, 150));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new FundsDepositedEvent(aggregateId, 100));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new FundsWithdrawedEvent(aggregateId, 60));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new FundsWithdrawedEvent(aggregateId, 94));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        await repository.AddEventToStream(streamName, new FundsDepositedEvent(aggregateId, 4));
        await eventHandlerFactory.HandleAllStreamEventRecordsAsync(streamName);
        Console.WriteLine();

        Console.WriteLine("");
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
                services.AddSingleton<IEventStoreRepository>(provider => new EventStoreRepository(esdbConnectionString));
                services.AddSingleton<IEventHandlerFactory, EventHandlerFactory>();
            });

        return builder;
    }
}


