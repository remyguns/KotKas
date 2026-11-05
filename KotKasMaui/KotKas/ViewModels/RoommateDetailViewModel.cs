using System.Collections.ObjectModel;
using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KotKas.Messages;
using KotKas.Models;
using KotKas.Services;

namespace KotKas.ViewModels;

public partial class RoommateDetailViewModel : ObservableRecipient, IRoommateDetailViewModel
{
    private readonly IRoommateDataService _roommateDataService;
    private readonly IExpenseDataService _expenseDataService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string title = "Nieuwe kotgenoot";

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isExistingRoommate;

    [ObservableProperty]
    private Roommate? currentRoommate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public ObservableCollection<Expense> PaidExpenses { get; } = new();

    public bool HasNoExpenses => PaidExpenses.Count == 0;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public RoommateDetailViewModel(IRoommateDataService roommateDataService,
                                   IExpenseDataService expenseDataService,
                                   INavigationService navigationService,
                                   IDialogService dialogService)
    {
        _roommateDataService = roommateDataService;
        _expenseDataService = expenseDataService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        Messenger.Register<RoommateDetailViewModel, RoommateSelectedMessage>(this, (r, m) => r.ShowRoommate(m.Value));
    }

    private void ShowRoommate(Roommate roommate)
    {
        CurrentRoommate = roommate;
        IsExistingRoommate = roommate.Id > 0;
        Title = IsExistingRoommate ? roommate.Name : "Nieuwe kotgenoot";
        Name = roommate.Name;
        Email = roommate.Email;
        ErrorMessage = string.Empty;
        PaidExpenses.Clear();
        OnPropertyChanged(nameof(HasNoExpenses));

        if (IsExistingRoommate)
            _ = LoadExpensesAsync(roommate.Id);
    }

    private async Task LoadExpensesAsync(int roommateId)
    {
        try
        {
            var expenses = await _expenseDataService.GetByRoommateAsync(roommateId);
            PaidExpenses.Clear();
            foreach (var expense in expenses)
                PaidExpenses.Add(expense);
            OnPropertyChanged(nameof(HasNoExpenses));
        }
        catch (Exception)
        {
            ErrorMessage = "De uitgaven konden niet geladen worden.";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = Validate();
        if (HasError)
            return;

        var roommate = new Roommate
        {
            Id = CurrentRoommate?.Id ?? 0,
            Name = Name.Trim(),
            Email = Email.Trim()
        };

        try
        {
            bool success = IsExistingRoommate
                ? await _roommateDataService.PutAsync(roommate.Id, roommate)
                : await _roommateDataService.PostAsync(roommate) != null;

            if (!success)
            {
                ErrorMessage = "Opslaan mislukt.";
                return;
            }

            Messenger.Send(new RefreshRoommatesMessage(true));
            await _navigationService.NavigateBackAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Opslaan mislukt. Draait de API?";
        }
    }

    private string Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "Vul een naam in.";

        if (!string.IsNullOrWhiteSpace(Email) && !MailAddress.TryCreate(Email.Trim(), out _))
            return "Vul een geldig e-mailadres in.";

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
        if (!IsExistingRoommate || CurrentRoommate == null)
            return;

        bool confirmed = await _dialogService.ShowConfirmationAsync("Verwijderen",
            $"{CurrentRoommate.Name} verwijderen? Ook de uitgaven die hij betaald heeft worden verwijderd.");
        if (!confirmed)
            return;

        try
        {
            if (await _roommateDataService.DeleteAsync(CurrentRoommate.Id))
            {
                Messenger.Send(new RefreshRoommatesMessage(true));
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
