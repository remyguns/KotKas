using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KotKas.Messages;
using KotKas.Models;
using KotKas.Services;

namespace KotKas.ViewModels;

public partial class RoommatesViewModel : ObservableRecipient, IRoommatesViewModel
{
    private readonly IRoommateDataService _roommateDataService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Roommate? selectedRoommate;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public ObservableCollection<Roommate> Roommates { get; } = new();

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public RoommatesViewModel(IRoommateDataService roommateDataService, INavigationService navigationService)
    {
        _roommateDataService = roommateDataService;
        _navigationService = navigationService;

        // Saldo's veranderen na elke nieuwe of verwijderde uitgave
        Messenger.Register<RoommatesViewModel, RefreshExpensesMessage>(this, async (r, m) => await r.LoadRoommatesAsync());
        Messenger.Register<RoommatesViewModel, RefreshRoommatesMessage>(this, async (r, m) => await r.LoadRoommatesAsync());

        _ = LoadRoommatesAsync();
    }

    [RelayCommand]
    private async Task LoadRoommatesAsync()
    {
        try
        {
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            var roommates = await _roommateDataService.GetAllAsync();
            Roommates.Clear();
            foreach (var roommate in roommates.OrderByDescending(r => r.Balance))
                Roommates.Add(roommate);
        }
        catch (Exception)
        {
            ErrorMessage = "De kotgenoten konden niet geladen worden. Draait de API?";
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task AddRoommateAsync()
    {
        await _navigationService.NavigateToRoommateDetailPageAsync();
        Messenger.Send(new RoommateSelectedMessage(new Roommate()));
    }

    partial void OnSelectedRoommateChanged(Roommate? value)
    {
        if (value == null)
            return;

        _ = OpenRoommateAsync(value);
        SelectedRoommate = null;
    }

    private async Task OpenRoommateAsync(Roommate roommate)
    {
        await _navigationService.NavigateToRoommateDetailPageAsync();
        Messenger.Send(new RoommateSelectedMessage(roommate));
    }
}
