using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MachineEvidency.AuthService;
using MachineEvidency.Data;
using MachineEvidency.ViewModels;
using MachineEvidency.Views;

namespace MachineEvidency;

public class App : Application
{

    private DataService _dataService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dbFilePath = "/Users/martinbjalon/Desktop/EvidencyApp/MachineEvidency/machines.db";
        _dataService = new DataService(dbFilePath);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginWindow = new LoginView();
            loginWindow.DataContext = new LoginViewModel(_dataService);
            desktop.MainWindow = loginWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public void CloseLoginWindow()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        foreach (var window in desktop.Windows)
        {
            if (window is not LoginView) continue;
            window.Close();
            break;
        }
    }
    
    public void OpenLoginWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginView = new LoginView();
            loginView.DataContext = new LoginViewModel(_dataService); 
            desktop.MainWindow = loginView;
            desktop.MainWindow.Show();
        }
    }
    
    public void OpenMainWindow(User user)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow(_dataService);
            var mainViewModel = new MainViewModel(_dataService) { LoggedInUser = user };
            mainWindow.DataContext = mainViewModel; 
            desktop.MainWindow = mainWindow;
            desktop.MainWindow.Show();
        }
    }
    
    public void CloseMainWindow()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        foreach (var window in desktop.Windows)
        {
            if (window is not MainWindow) continue;
            window.Close();
            break;
        }
    }
}