using CommunityToolkit.Mvvm.Messaging.Messages;
using KotKas.Models;

namespace KotKas.Messages;

// RoommatesViewModel -> RoommateDetailViewModel: kotgenoot (nieuw of bestaand)
public class RoommateSelectedMessage : ValueChangedMessage<Roommate>
{
    public RoommateSelectedMessage(Roommate value) : base(value)
    {
    }
}
