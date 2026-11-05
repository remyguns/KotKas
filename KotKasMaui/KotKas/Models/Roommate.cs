using CommunityToolkit.Mvvm.ComponentModel;

namespace KotKas.Models;

public partial class Roommate : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Initials))]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPaidText))]
    private decimal totalPaid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCreditor))]
    [NotifyPropertyChangedFor(nameof(BalanceText))]
    private decimal balance;

    // Groen: krijgt geld terug, rood: moet betalen
    public bool IsCreditor => Balance >= 0;

    public string BalanceText => Balance >= 0
        ? $"krijgt € {Balance:0.00} terug"
        : $"moet € {Math.Abs(Balance):0.00} betalen";

    public string TotalPaidText => $"Betaalde in totaal € {TotalPaid:0.00}";

    // Picker toont de naam
    public override string ToString() => Name;

    public string Initials => string.IsNullOrWhiteSpace(Name)
        ? "?"
        : string.Concat(Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpper(p[0])));
}
