using CommunityToolkit.Mvvm.ComponentModel;

namespace KotKas.Models;

public partial class Expense : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StoreInitial))]
    private string storeName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PurchaseDateText))]
    private DateTime purchaseDate = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAmountText))]
    private decimal totalAmount;

    [ObservableProperty]
    private int roommateId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PaidByText))]
    private string roommateName = string.Empty;

    public string StoreInitial => string.IsNullOrWhiteSpace(StoreName) ? "?" : StoreName.Trim()[..1].ToUpper();
    public string PurchaseDateText => PurchaseDate.ToString("dd/MM/yyyy");
    public string TotalAmountText => $"€ {TotalAmount:0.00}";
    public string PaidByText => $"Betaald door {RoommateName}";
}
