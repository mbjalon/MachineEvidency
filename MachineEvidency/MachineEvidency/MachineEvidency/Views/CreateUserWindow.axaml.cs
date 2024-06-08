using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MachineEvidency.Data;
using MachineEvidency.ViewModels;

namespace MachineEvidency.Views;

public partial class CreateUserWindow : Window
{
    public CreateUserWindow(DataService dataService)
    {
        InitializeComponent();
        DataContext = new CreateUserViewModel(dataService);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}