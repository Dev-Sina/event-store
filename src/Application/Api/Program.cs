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

        Guid aggregateId = Guid.NewGuid();

        string streamName1 = await eventPublisher.PublishEventAsync(new AccountCreatedEvent(aggregateId, "Sina Bahmanpour"));
        await eventSubscriber.SubscribeEventAsync(streamName1);

        string streamName2 = await eventPublisher.PublishEventAsync(new FundsDepositedEvent(aggregateId, 150));
        await eventSubscriber.SubscribeEventAsync(streamName2);

        string streamName3 = await eventPublisher.PublishEventAsync(new FundsDepositedEvent(aggregateId, 100));
        await eventSubscriber.SubscribeEventAsync(streamName3);

        string streamName4 = await eventPublisher.PublishEventAsync(new FundsWithdrawedEvent(aggregateId, 60));
        await eventSubscriber.SubscribeEventAsync(streamName4);

        string streamName5 = await eventPublisher.PublishEventAsync(new FundsWithdrawedEvent(aggregateId, 94));
        await eventSubscriber.SubscribeEventAsync(streamName5);

        string streamName6 = await eventPublisher.PublishEventAsync(new FundsDepositedEvent(aggregateId, 4));
        await eventSubscriber.SubscribeEventAsync(streamName6);

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
                services.AddSingleton<BankAccountModel>();
                services.AddTransient<TransactionModel>();
                services.AddScoped<IEventHandler<AccountCreatedEvent>, AccountCreatedEventHandler>();
                services.AddScoped<IEventHandler<FundsDepositedEvent>, FundsDepositedEventHandler>();
                services.AddScoped<IEventHandler<FundsWithdrawedEvent>, FundsWithdrawedEventHandler>();
                services.AddSingleton<IEventPublisher, EventPublisher>();
                services.AddSingleton<IEventSubscriber, EventSubscriber>();
            });

        return builder;
    }
}


