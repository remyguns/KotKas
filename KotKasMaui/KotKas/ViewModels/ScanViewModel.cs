using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KotKas.Helpers;
using KotKas.Messages;
using KotKas.Models;
using KotKas.Services;

namespace KotKas.ViewModels;

public partial class ScanViewModel : ObservableRecipient, IScanViewModel
{
    private readonly IReceiptAnalyzerService _receiptAnalyzerService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private string? imagePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private bool isBusy;

    [ObservableProperty]
    private string statusText = "Neem een foto van je kasticket of kies er een uit je galerij.";

    public bool HasImage => !string.IsNullOrEmpty(ImagePath);

    public ScanViewModel(IReceiptAnalyzerService receiptAnalyzerService,
                         INavigationService navigationService,
                         IDialogService dialogService)
    {
        _receiptAnalyzerService = receiptAnalyzerService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        // Na het opslaan van een uitgave: klaar voor het volgende ticket
        Messenger.Register<ScanViewModel, RefreshExpensesMessage>(this, (r, m) => r.Reset());
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await _dialogService.ShowMessageAsync("Camera", "Op dit toestel kan je geen foto nemen. Kies een foto uit de galerij.");
            return;
        }

        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await _dialogService.ShowMessageAsync("Camera", "KotKas heeft toegang tot de camera nodig om een ticket te scannen.");
            return;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        await SavePhotoAsync(photo);
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        var photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 });
        await SavePhotoAsync(photos?.FirstOrDefault());
    }

    private async Task SavePhotoAsync(FileResult? photo)
    {
        if (photo == null)
            return;

        // Kopie in de cache, zodat we het bestand zeker kunnen lezen
        var localPath = Path.Combine(FileSystem.CacheDirectory, $"ticket_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(photo.FileName)}");
        await using (var source = await photo.OpenReadAsync())
        await using (var target = File.Create(localPath))
        {
            await source.CopyToAsync(target);
        }

        ImagePath = localPath;
        StatusText = "Klaar om te analyseren.";
    }

    private bool CanAnalyze() => HasImage && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private async Task AnalyzeAsync()
    {
        if (!Constants.IsAzureConfigured)
        {
            await _dialogService.ShowMessageAsync("Azure", "Vul eerst de Azure-sleutels in Helpers/Constants.cs in.");
            return;
        }

        try
        {
            IsBusy = true;
            StatusText = "Ticket wordt geanalyseerd...";

            var receipt = await _receiptAnalyzerService.AnalyzeAsync(ImagePath!);

            if (receipt.Lines.Count == 0)
            {
                StatusText = "Er werd geen tekst gevonden. Probeer een scherpere foto.";
                return;
            }

            await _navigationService.NavigateToReviewExpensePageAsync();
            Messenger.Send(new ReceiptScannedMessage(receipt));
            StatusText = "Controleer de gegevens en sla de uitgave op.";
        }
        catch (Exception ex)
        {
            StatusText = "Analyseren mislukt.";
            await _dialogService.ShowMessageAsync("Fout", $"Het ticket kon niet geanalyseerd worden.\n{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddManuallyAsync()
    {
        await _navigationService.NavigateToReviewExpensePageAsync();
        Messenger.Send(new ReceiptScannedMessage(new ScannedReceipt()));
    }

    private void Reset()
    {
        ImagePath = null;
        StatusText = "Neem een foto van je kasticket of kies er een uit je galerij.";
    }
}
