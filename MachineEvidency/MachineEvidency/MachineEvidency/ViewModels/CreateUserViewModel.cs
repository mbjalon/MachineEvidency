using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MachineEvidency.AuthService;
using MachineEvidency.Data;
using ReactiveUI;

namespace MachineEvidency.ViewModels;

public class CreateUserViewModel : ViewModelBase
{
    private readonly DataService _databaseService;
    private string _username;
    private string _password;
    private string _confirmPassword;
    private string _role;
    private string _errorMessage;
    private string _successMessage;

    public CreateUserViewModel(DataService dataService)
    {
        _databaseService = dataService;
        CreateUserCommand = new AsyncRelayCommand(CreateUserAsync);
    }

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

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    public string Role
    {
        get => _role;
        set => this.RaiseAndSetIfChanged(ref _role, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    
    public string SuccessMessage
    {
        get => _successMessage;
        set => this.RaiseAndSetIfChanged(ref _successMessage, value);
    }

    public ICommand CreateUserCommand { get; }

    private async Task CreateUserAsync()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Heslá sa nezhodujú";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");
            return;
        }

        var existingUser = await _databaseService.GetUserByUsernameAsync(Username);
        if (existingUser != null)
        {
            ErrorMessage = $"Používateľ {existingUser.Username} už existuje";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");
            return;
        }

        var role = _role switch
        {
            "Admin" => UserRole.Admin,
            "Superuser" => UserRole.SuperUser,
            _ => UserRole.User
        };
        
        var newUser = new User
        {
            Username = Username,
            Password = Password,
            Role = role
        };

        await _databaseService.AddUserAsync(newUser);

        SuccessMessage = $"Používateľ {newUser.Username} úspešne zaregisrovaný";
        Observable.Timer(TimeSpan.FromSeconds(10))
            .Subscribe(_ => SuccessMessage = "");
    }
}