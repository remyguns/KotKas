using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KotKas.Messages;

// Na opslaan/verwijderen: lijsten met uitgaven en saldo's vernieuwen
public class RefreshExpensesMessage : ValueChangedMessage<bool>
{
    public RefreshExpensesMessage(bool value) : base(value)
    {
    }
}
