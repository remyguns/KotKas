using KotKas.Views;

namespace KotKas.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static INavigation Navigation => Shell.Current.Navigation;

    // De serviceprovider geeft de pagina: geen harde afhankelijkheid tussen View en ViewModel
    public async Task NavigateToReviewExpensePageAsync()
    {
        var page = _serviceProvider.GetRequiredService<ReviewExpensePage>();
        await Navigation.PushAsync(page);
    }

    public async Task NavigateToRoommateDetailPageAsync()
    {
        var page = _serviceProvider.GetRequiredService<RoommateDetailPage>();
        await Navigation.PushAsync(page);
    }

    public async Task NavigateBackAsync()
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
    }
}
