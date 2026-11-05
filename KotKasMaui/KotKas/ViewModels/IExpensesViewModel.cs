using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using KotKas.Models;

namespace KotKas.ViewModels;

public interface IExpensesViewModel
{
    ObservableCollection<Expense> Expenses { get; }
    Expense? SelectedExpense { get; set; }
    bool IsRefreshing { get; set; }
    string TotalText { get; set; }
    string ErrorMessage { get; set; }
    bool HasError { get; }

    IAsyncRelayCommand LoadExpensesCommand { get; }
}
