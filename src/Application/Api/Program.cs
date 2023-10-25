using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using ESDB.Core.Models;
using ESDB.Services.Publisher;
using ESDB.Services.Subscription;
using ESDB.Services.Subscription.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ESDB.Application.Api;

public static class Program
{
    public static async Task Main(params string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        var eventPublisher = host.Services.GetRequiredService<IEventPublisher>();
        var eventSubscriber = host.Services.GetRequiredService<IEventSubscriber>();

        string streamName = $"sina-{DateTime.UtcNow}";
        Guid aggregateId = Guid.NewGuid();

        await eventPublisher.PublishEventAsync(streamName, new AccountCreatedEvent(aggregateId, "Sina Bahmanpour"));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        await eventPublisher.PublishEventAsync(streamName, new FundsDepositedEvent(aggregateId, 150));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        await eventPublisher.PublishEventAsync(streamName, new FundsDepositedEvent(aggregateId, 100));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        await eventPublisher.PublishEventAsync(streamName, new FundsWithdrawedEvent(aggregateId, 60));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        await eventPublisher.PublishEventAsync(streamName, new FundsWithdrawedEvent(aggregateId, 94));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        await eventPublisher.PublishEventAsync(streamName, new FundsDepositedEvent(aggregateId, 4));
        await eventSubscriber.SubscribeEventAsync(streamName);
        Console.WriteLine();

        Console.WriteLine("");
        Console.Write("Please press a key to exit...");
        Console.ReadLine();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
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
                services.AddSingleton<IEventPublisher, EventPublisher>();
                services.AddSingleton<IEventSubscriber, EventSubscriber>();
            });

        return builder;
    }
}


