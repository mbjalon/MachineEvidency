using System.Reactive;
using Avalonia;
using MachineEvidency.Data;
using ReactiveUI;

namespace MachineEvidency.ViewModels;

public class LoginViewModel : ReactiveObject
{
    private string _username;
    private string _password;
    private string _errorMessage;
    private readonly DataService _databaseService;

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public LoginViewModel(DataService dataService)
    {
        LoginCommand = ReactiveCommand.Create(Login);
        _databaseService = dataService;
    }

    public async void Login()
    {
        var user = await _databaseService.GetUserByUsernameAsync(Username);
        
        if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.Password))
        {
            (Application.Current as App)?.OpenMainWindow(user);
            (Application.Current as App)?.CloseLoginWindow();
        }
        else
        {
            ErrorMessage = "Invalid username or password";
        }
    }
}