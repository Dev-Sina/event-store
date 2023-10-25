using System.Text;
using System.Text.Json;
using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using EventStore.Client;

namespace ESDB.Services.Subscription;

public class EventSubscriber : IEventSubscriber
{
    private readonly string _connectionString;

    private readonly EventStoreClient _client;
    
    private readonly IEventHandler<AccountCreatedEvent> _accountCreatedEventHandler;
    private readonly IEventHandler<FundsDepositedEvent> _fundsDepositedEventHandler;
    private readonly IEventHandler<FundsWithdrawedEvent> _fundsWithdrawedEventHandler;

    public EventSubscriber(IEventHandler<AccountCreatedEvent> accountCreatedEventHandler,
        IEventHandler<FundsDepositedEvent> fundsDepositedEventHandler,
        IEventHandler<FundsWithdrawedEvent> fundsWithdrawedEventHandler)
    {
        _connectionString = "esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";

        var settings = EventStoreClientSettings.Create($"{_connectionString}");

        _client = new EventStoreClient(settings);
        _accountCreatedEventHandler = accountCreatedEventHandler;
        _fundsDepositedEventHandler = fundsDepositedEventHandler;
        _fundsWithdrawedEventHandler = fundsWithdrawedEventHandler;
    }

    private async Task SubscribeResolvedEventsAsync(List<ResolvedEvent> resolvedEvents,
        CancellationToken cancellationToken = default)
    {
        resolvedEvents
            .ForEach(async x =>
            {
                var eventByteArrayData = x.Event.Data;
                var eventSerializedData = Encoding.UTF8.GetString(eventByteArrayData.ToArray());
                IEvent? eventObject = null;
                switch (x.Event.EventType)
                {
                    case nameof(AccountCreatedEvent):
                        eventObject = JsonSerializer.Deserialize<AccountCreatedEvent>(eventSerializedData);
                        if (eventObject is AccountCreatedEvent accountCreatedEvent)
                        {
                            await _accountCreatedEventHandler.HandleEventAsync(accountCreatedEvent);
                        }
                        break;
                    case nameof(FundsDepositedEvent):
                        eventObject = JsonSerializer.Deserialize<FundsDepositedEvent>(eventSerializedData);
                        if (eventObject is FundsDepositedEvent fundsDepositedEvent)
                        {
                            await _fundsDepositedEventHandler.HandleEventAsync(fundsDepositedEvent);
                        }
                        break;
                    case nameof(FundsWithdrawedEvent):
                        eventObject = JsonSerializer.Deserialize<FundsWithdrawedEvent>(eventSerializedData);
                        if (eventObject is FundsWithdrawedEvent fundsWithdrawedEvent)
                        {
                            await _fundsWithdrawedEventHandler.HandleEventAsync(fundsWithdrawedEvent);
                        }
                        break;
                    default:
                        break;
                }
            });

        await Task.CompletedTask;

        return;
    }

    public async Task SubscribeEventAsync(string streamName, CancellationToken cancellationToken = default)
    {
        var result = _client.ReadStreamAsync(Direction.Forwards,
            streamName,
            StreamPosition.Start,
            cancellationToken: default);
        if (result is null)
        {
            return;
        }

        List<ResolvedEvent> resolvedEvents = await result.ToListAsync(default);
        if (resolvedEvents is null ||
            !resolvedEvents.Any())
        {
            return;
        }

        await SubscribeResolvedEventsAsync(resolvedEvents, cancellationToken);
    }
}
