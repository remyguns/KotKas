using KotKas.Services;
using KotKas.ViewModels;
using KotKas.Views;
using Microsoft.Extensions.Logging;

namespace KotKas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IRoommateDataService, RoommateDataService>();
        builder.Services.AddSingleton<IExpenseDataService, ExpenseDataService>();
        builder.Services.AddSingleton<IComputerVisionService, ComputerVisionService>();
        builder.Services.AddSingleton<ICustomVisionService, CustomVisionService>();
        builder.Services.AddSingleton<IReceiptParser, ReceiptParser>();
        builder.Services.AddSingleton<IReceiptAnalyzerService, ReceiptAnalyzerService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        builder.Services.AddSingleton<IScanViewModel, ScanViewModel>();
        builder.Services.AddSingleton<IReviewExpenseViewModel, ReviewExpenseViewModel>();
        builder.Services.AddSingleton<IExpensesViewModel, ExpensesViewModel>();
        builder.Services.AddSingleton<IRoommatesViewModel, RoommatesViewModel>();
        builder.Services.AddSingleton<IRoommateDetailViewModel, RoommateDetailViewModel>();

        // Views
        builder.Services.AddSingleton<ScanPage>();
        builder.Services.AddSingleton<ExpensesPage>();
        builder.Services.AddSingleton<RoommatesPage>();
        builder.Services.AddTransient<ReviewExpensePage>();
        builder.Services.AddTransient<RoommateDetailPage>();

        return builder.Build();
    }
}
