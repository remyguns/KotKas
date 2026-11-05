using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using KotKas.Models;

namespace KotKas.ViewModels;

public interface IRoommatesViewModel
{
    ObservableCollection<Roommate> Roommates { get; }
    Roommate? SelectedRoommate { get; set; }
    bool IsRefreshing { get; set; }
    string ErrorMessage { get; set; }
    bool HasError { get; }

    IAsyncRelayCommand LoadRoommatesCommand { get; }
    IAsyncRelayCommand AddRoommateCommand { get; }
}
