using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using KotKas.Models;

namespace KotKas.ViewModels;

public interface IReviewExpenseViewModel
{
    string Title { get; set; }
    string StoreName { get; set; }
    DateTime PurchaseDate { get; set; }
    string TotalAmount { get; set; }
    ObservableCollection<Roommate> Roommates { get; }
    Roommate? SelectedRoommate { get; set; }

    string? ImagePath { get; set; }
    bool HasImage { get; }
    string ScanInfo { get; set; }
    bool HasScanInfo { get; }
    bool IsExistingExpense { get; set; }

    string ErrorMessage { get; set; }
    bool HasError { get; }
    bool IsBusy { get; set; }

    IAsyncRelayCommand SaveCommand { get; }
    IAsyncRelayCommand CancelCommand { get; }
    IAsyncRelayCommand DeleteCommand { get; }
}
