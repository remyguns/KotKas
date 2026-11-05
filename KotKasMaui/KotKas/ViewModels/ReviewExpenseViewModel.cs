using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KotKas.Messages;
using KotKas.Models;
using KotKas.Services;

namespace KotKas.ViewModels;

public partial class ReviewExpenseViewModel : ObservableRecipient, IReviewExpenseViewModel
{
    private readonly IExpenseDataService _expenseDataService;
    private readonly IRoommateDataService _roommateDataService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private int _expenseId;

    [ObservableProperty]
    private string title = "Nieuwe uitgave";

    [ObservableProperty]
    private string storeName = string.Empty;

    [ObservableProperty]
    private DateTime purchaseDate = DateTime.Today;

    [ObservableProperty]
    private string totalAmount = string.Empty;

    [ObservableProperty]
    private Roommate? selectedRoommate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    private string? imagePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasScanInfo))]
    private string scanInfo = string.Empty;

    [ObservableProperty]
    private bool isExistingExpense;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<Roommate> Roommates { get; } = new();

    public bool HasImage => !string.IsNullOrEmpty(ImagePath);
    public bool HasScanInfo => !string.IsNullOrEmpty(ScanInfo);
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ReviewExpenseViewModel(IExpenseDataService expenseDataService,
                                  IRoommateDataService roommateDataService,
                                  INavigationService navigationService,
                                  IDialogService dialogService)
    {
        _expenseDataService = expenseDataService;
        _roommateDataService = roommateDataService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        // Nieuw ticket vanuit ScanPage
        Messenger.Register<ReviewExpenseViewModel, ReceiptScannedMessage>(this, (r, m) => r.ShowScannedReceipt(m.Value));
        // Bestaande uitgave vanuit ExpensesPage
        Messenger.Register<ReviewExpenseViewModel, ExpenseSelectedMessage>(this, (r, m) => r.ShowExistingExpense(m.Value));
    }

    private void ShowScannedReceipt(ScannedReceipt receipt)
    {
        _expenseId = 0;
        IsExistingExpense = false;
        Title = "Nieuwe uitgave";
        StoreName = receipt.StoreName;
        PurchaseDate = receipt.PurchaseDate;
        TotalAmount = receipt.TotalAmount > 0 ? receipt.TotalAmount.ToString("0.00", CultureInfo.GetCultureInfo("nl-BE")) : string.Empty;
        ImagePath = string.IsNullOrEmpty(receipt.ImagePath) ? null : receipt.ImagePath;
        ErrorMessage = string.Empty;
        ScanInfo = BuildScanInfo(receipt);

        _ = LoadRoommatesAsync(null);
    }

    private void ShowExistingExpense(Expense expense)
    {
        _expenseId = expense.Id;
        IsExistingExpense = true;
        Title = "Uitgave aanpassen";
        StoreName = expense.StoreName;
        PurchaseDate = expense.PurchaseDate;
        TotalAmount = expense.TotalAmount.ToString("0.00", CultureInfo.GetCultureInfo("nl-BE"));
        ImagePath = null;
        ScanInfo = string.Empty;
        ErrorMessage = string.Empty;

        _ = LoadRoommatesAsync(expense.RoommateId);
    }

    private static string BuildScanInfo(ScannedReceipt receipt)
    {
        if (receipt.Lines.Count == 0)
            return string.Empty;

        var info = receipt.DetectedStore == Store.Other
            ? "Winkel niet herkend"
            : receipt.StoreProbability > 0
                ? $"Winkel herkend: {receipt.DetectedStore} ({receipt.StoreProbability:P0})"
                : $"Winkel gevonden in de tekst: {receipt.DetectedStore}";

        if (!receipt.TotalFound)
            info += " · totaal niet gevonden";
        if (!receipt.DateFound)
            info += " · datum niet gevonden";

        return info + ". Controleer de gegevens.";
    }

    private async Task LoadRoommatesAsync(int? roommateId)
    {
        try
        {
            var roommates = await _roommateDataService.GetAllAsync();
            Roommates.Clear();
            foreach (var roommate in roommates)
                Roommates.Add(roommate);

            SelectedRoommate = roommateId.HasValue
                ? Roommates.FirstOrDefault(r => r.Id == roommateId.Value)
                : null;
        }
        catch (Exception)
        {
            ErrorMessage = "De kotgenoten konden niet geladen worden. Draait de API?";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = Validate(out var amount);
        if (HasError)
            return;

        var expense = new Expense
        {
            Id = _expenseId,
            StoreName = StoreName.Trim(),
            PurchaseDate = PurchaseDate.Date,
            TotalAmount = amount,
            RoommateId = SelectedRoommate!.Id
        };

        try
        {
            IsBusy = true;
            bool success = IsExistingExpense
                ? await _expenseDataService.PutAsync(_expenseId, expense)
                : await _expenseDataService.PostAsync(expense) != null;

            if (!success)
            {
                ErrorMessage = "Opslaan mislukt.";
                return;
            }

            Messenger.Send(new RefreshExpensesMessage(true));
            await _navigationService.NavigateBackAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Opslaan mislukt. Draait de API?";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string Validate(out decimal amount)
    {
        amount = 0;
        if (string.IsNullOrWhiteSpace(StoreName))
            return "Vul de winkel in.";

        var normalized = TotalAmount.Replace("€", string.Empty).Replace(',', '.').Trim();
        if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) || amount <= 0)
            return "Vul een geldig bedrag in (bv. 23,45).";

        if (PurchaseDate.Date > DateTime.Today)
            return "De datum kan niet in de toekomst liggen.";

        if (SelectedRoommate == null)
            return "Kies wie betaald heeft.";

        return string.Empty;
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.NavigateBackAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (!IsExistingExpense)
            return;

        bool confirmed = await _dialogService.ShowConfirmationAsync("Verwijderen", $"Uitgave bij {StoreName} verwijderen?");
        if (!confirmed)
            return;

        try
        {
            if (await _expenseDataService.DeleteAsync(_expenseId))
            {
                Messenger.Send(new RefreshExpensesMessage(true));
                await _navigationService.NavigateBackAsync();
            }
            else
            {
                ErrorMessage = "Verwijderen mislukt.";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Verwijderen mislukt. Draait de API?";
        }
    }
}
