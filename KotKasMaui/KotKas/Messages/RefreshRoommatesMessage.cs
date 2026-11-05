using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KotKas.Messages;

// Na toevoegen/aanpassen/verwijderen van een kotgenoot: lijsten vernieuwen
public class RefreshRoommatesMessage : ValueChangedMessage<bool>
{
    public RefreshRoommatesMessage(bool value) : base(value)
    {
    }
}
