namespace KotKas.Services;

// Zo hoeven de ViewModels geen Page te kennen om een melding te tonen
public class DialogService : IDialogService
{
    public async Task ShowMessageAsync(string title, string message)
    {
        await Shell.Current.DisplayAlertAsync(title, message, "OK");
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await Shell.Current.DisplayAlertAsync(title, message, "Ja", "Nee");
    }
}
