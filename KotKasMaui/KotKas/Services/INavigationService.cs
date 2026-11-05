namespace KotKas.Services;

// ViewModels navigeren via deze interface, zonder de Views te kennen
public interface INavigationService
{
    Task NavigateToReviewExpensePageAsync();
    Task NavigateToRoommateDetailPageAsync();
    Task NavigateBackAsync();
}
