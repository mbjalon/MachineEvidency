using Avalonia.Controls;
using MachineEvidency.Data;
using MachineEvidency.ViewModels;

namespace MachineEvidency.Views;

public partial class AddMachineWindow : Window
{
    public AddMachineWindow(DataService dataService)
    {
        InitializeComponent();
        DataContext = new AddMachineViewModel(dataService);
    }
}