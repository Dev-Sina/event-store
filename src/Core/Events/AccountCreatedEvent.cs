namespace ESDB.Core.Events;

public class AccountCreatedEvent : IEvent
{
    public AccountCreatedEvent(Guid guid, string name)
    {
        Guid = guid;
        Name = name;
    }

    public Guid Guid { get; private set; }
    public string Name { get; private set; }
}
