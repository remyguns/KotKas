using CommunityToolkit.Mvvm.Messaging.Messages;
using KotKas.Models;

namespace KotKas.Messages;

// ScanViewModel -> ReviewExpenseViewModel: het uitgelezen kasticket
public class ReceiptScannedMessage : ValueChangedMessage<ScannedReceipt>
{
    public ReceiptScannedMessage(ScannedReceipt value) : base(value)
    {
    }
}
