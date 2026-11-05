using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using KotKas.Models;

namespace KotKas.ViewModels;

public interface IRoommateDetailViewModel
{
    string Title { get; set; }
    string Name { get; set; }
    string Email { get; set; }
    bool IsExistingRoommate { get; set; }
    Roommate? CurrentRoommate { get; set; }
    ObservableCollection<Expense> PaidExpenses { get; }
    bool HasNoExpenses { get; }

    string ErrorMessage { get; set; }
    bool HasError { get; }

    IAsyncRelayCommand SaveCommand { get; }
    IAsyncRelayCommand CancelCommand { get; }
    IAsyncRelayCommand DeleteCommand { get; }
}
