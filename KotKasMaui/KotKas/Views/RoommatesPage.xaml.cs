using KotKas.ViewModels;

namespace KotKas.Views;

public partial class RoommatesPage : ContentPage
{
    public RoommatesPage(IRoommatesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
