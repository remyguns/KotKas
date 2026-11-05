namespace KotKas.Services;

public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
    Task<bool> ShowConfirmationAsync(string title, string message);
}
