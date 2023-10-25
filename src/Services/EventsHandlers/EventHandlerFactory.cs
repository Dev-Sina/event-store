using ESDB.Core.Abstraction;
using ESDB.Core.Events;
using EventStore.Client;
using System.Text;

namespace ESDB.Services.EventsHandlers;

public class EventHandlerFactory : IEventHandlerFactory
{
    private readonly IEventHandler<AccountCreatedEvent> _accountCreatedEventHandler;
    private readonly IEventHandler<FundsDepositedEvent> _fundsDepositedEventHandler;
    private readonly IEventHandler<FundsWithdrawedEvent> _fundsWithdrawedEventHandler;
    private readonly IEventStoreRepository _eventStoreRepository;

    public EventHandlerFactory(IEventHandler<AccountCreatedEvent> accountCreatedEventHandler,
        IEventHandler<FundsDepositedEvent> fundsDepositedEventHandler,
        IEventHandler<FundsWithdrawedEvent> fundsWithdrawedEventHandler,
        IEventStoreRepository eventStoreRepository)
    {
        _accountCreatedEventHandler = accountCreatedEventHandler;
        _fundsDepositedEventHandler = fundsDepositedEventHandler;
        _fundsWithdrawedEventHandler = fundsWithdrawedEventHandler;
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task HandleEventAsync(IEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (@event is null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        switch (@event.GetType().Name)
        {
            case nameof(AccountCreatedEvent):
                await _accountCreatedEventHandler.HandleEventAsync((AccountCreatedEvent)@event, cancellationToken);
                break;
            case nameof(FundsDepositedEvent):
                await _fundsDepositedEventHandler.HandleEventAsync((FundsDepositedEvent)@event, cancellationToken);
                break;
            case nameof(FundsWithdrawedEvent):
                await _fundsWithdrawedEventHandler.HandleEventAsync((FundsWithdrawedEvent)@event, cancellationToken);
                break;
            default:
                break;
        }

        await Task.CompletedTask;

        return;
    }

    public async Task HandleEventsAsync(IEnumerable<IEvent> events,
        CancellationToken cancellationToken = default)
    {
        if (events is null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        foreach (var @event in events)
        {
            await HandleEventAsync(@event, cancellationToken);
        }
    }

    public async Task HandleEventRecordAsync(EventRecord eventRecord,
        CancellationToken cancellationToken = default)
    {
        if (eventRecord is null)
        {
            throw new ArgumentNullException(nameof(eventRecord));
        }

        IEvent? eventObject = null;
        var eventByteArrayData = eventRecord.Data;
        var eventSerializedData = Encoding.UTF8.GetString(eventByteArrayData.ToArray());

        string eventTypeName = eventRecord.EventType;
        switch (eventTypeName)
        {
            case nameof(AccountCreatedEvent):
                eventObject = System.Text.Json.JsonSerializer.Deserialize<AccountCreatedEvent>(eventSerializedData);
                await _accountCreatedEventHandler.HandleEventAsync((AccountCreatedEvent)eventObject!, cancellationToken);
                break;
            case nameof(FundsDepositedEvent):
                eventObject = System.Text.Json.JsonSerializer.Deserialize<FundsDepositedEvent>(eventSerializedData);
                await _fundsDepositedEventHandler.HandleEventAsync((FundsDepositedEvent)eventObject!, cancellationToken);
                break;
            case nameof(FundsWithdrawedEvent):
                eventObject = System.Text.Json.JsonSerializer.Deserialize<FundsWithdrawedEvent>(eventSerializedData);
                await _fundsWithdrawedEventHandler.HandleEventAsync((FundsWithdrawedEvent)eventObject!, cancellationToken);
                break;
            default:
                break;
        }

        await Task.CompletedTask;
        return;
    }

    public async Task HandleEventRecordsAsync(IEnumerable<EventRecord> eventRecords,
        CancellationToken cancellationToken = default)
    {
        if (eventRecords is null)
        {
            throw new ArgumentNullException(nameof(eventRecords));
        }

        foreach (var eventRecord in eventRecords)
        {
            await HandleEventRecordAsync(eventRecord, cancellationToken);
        }
    }

    public async Task HandleAllStreamEventRecordsAsync(string streamName,
        CancellationToken cancellationToken = default)
    {
        var eventRecords = await _eventStoreRepository.GetStreamEventsAsync(streamName, cancellationToken);
        await HandleEventRecordsAsync(eventRecords, cancellationToken);
    }
}
