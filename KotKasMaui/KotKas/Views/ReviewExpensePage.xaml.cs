using KotKas.ViewModels;

namespace KotKas.Views;

public partial class ReviewExpensePage : ContentPage
{
    public ReviewExpensePage(IReviewExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
