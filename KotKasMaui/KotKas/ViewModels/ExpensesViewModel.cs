using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KotKas.Messages;
using KotKas.Models;
using KotKas.Services;

namespace KotKas.ViewModels;

public partial class ExpensesViewModel : ObservableRecipient, IExpensesViewModel
{
    private readonly IExpenseDataService _expenseDataService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Expense? selectedExpense;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string totalText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public ObservableCollection<Expense> Expenses { get; } = new();

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ExpensesViewModel(IExpenseDataService expenseDataService, INavigationService navigationService)
    {
        _expenseDataService = expenseDataService;
        _navigationService = navigationService;

        Messenger.Register<ExpensesViewModel, RefreshExpensesMessage>(this, async (r, m) => await r.LoadExpensesAsync());
        Messenger.Register<ExpensesViewModel, RefreshRoommatesMessage>(this, async (r, m) => await r.LoadExpensesAsync());

        _ = LoadExpensesAsync();
    }

    [RelayCommand]
    private async Task LoadExpensesAsync()
    {
        try
        {
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            // De API geeft de uitgaven al terug met de nieuwste eerst
            var expenses = await _expenseDataService.GetAllAsync();
            Expenses.Clear();
            foreach (var expense in expenses)
                Expenses.Add(expense);

            TotalText = $"{Expenses.Count} uitgaven · totaal € {Expenses.Sum(e => e.TotalAmount):0.00}";
        }
        catch (Exception)
        {
            ErrorMessage = "De uitgaven konden niet geladen worden. Draait de API?";
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // Een uitgave aantikken = aanpassen of verwijderen
    partial void OnSelectedExpenseChanged(Expense? value)
    {
        if (value == null)
            return;

        _ = OpenExpenseAsync(value);
        SelectedExpense = null;
    }

    private async Task OpenExpenseAsync(Expense expense)
    {
        await _navigationService.NavigateToReviewExpensePageAsync();
        Messenger.Send(new ExpenseSelectedMessage(expense));
    }
}
