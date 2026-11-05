using CommunityToolkit.Mvvm.Input;

namespace KotKas.ViewModels;

public interface IScanViewModel
{
    string? ImagePath { get; set; }
    bool HasImage { get; }
    bool IsBusy { get; set; }
    string StatusText { get; set; }

    IAsyncRelayCommand TakePhotoCommand { get; }
    IAsyncRelayCommand PickPhotoCommand { get; }
    IAsyncRelayCommand AnalyzeCommand { get; }
    IAsyncRelayCommand AddManuallyCommand { get; }
}
