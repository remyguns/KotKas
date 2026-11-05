using KotKas.ViewModels;

namespace KotKas.Views;

public partial class ExpensesPage : ContentPage
{
    public ExpensesPage(IExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
