using KotKas.ViewModels;

namespace KotKas.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage(IScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
