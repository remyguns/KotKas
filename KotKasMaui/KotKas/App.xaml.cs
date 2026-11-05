namespace KotKas;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell()) { Title = "KotKas" };

#if WINDOWS
        // Op Windows het venster even groot als een telefoon tonen
        window.Width = 450;
        window.Height = 1000;
        window.X = 60;
        window.Y = 10;
#endif

        return window;
    }
}
