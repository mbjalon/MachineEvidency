using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MachineEvidency.ViewModels;
using MachineEvidency.ViewModels;

namespace MachineEvidency.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }
    
    private void EnterLogin(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.Login();
        }
    }
}