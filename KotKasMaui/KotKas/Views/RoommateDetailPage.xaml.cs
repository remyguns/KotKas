using KotKas.ViewModels;

namespace KotKas.Views;

public partial class RoommateDetailPage : ContentPage
{
    public RoommateDetailPage(IRoommateDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
