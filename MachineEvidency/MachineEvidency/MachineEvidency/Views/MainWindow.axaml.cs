using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MachineEvidency.Data;
using MachineEvidency.ViewModels;

namespace MachineEvidency.Views;

public partial class MainWindow : Window
{

    private readonly DataService _dataService;
    
    public MainWindow(DataService dataService)
    {
        _dataService = dataService;
        DataContext = new MainViewModel(_dataService);
        InitializeComponent();
    }
    
    private async void AddMachine(object sender, RoutedEventArgs e)
    {
        var addMachineWindow = new AddMachineWindow(_dataService);
        await addMachineWindow.ShowDialog(this);
        await ((MainViewModel)DataContext).LoadDataAsync();
    }
    
    private async void UpdateSmall(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit) return;
        if (e.Row.DataContext is not SmallMachine editedMachine)
            return; 
            
        var columnName = e.Column.Header.ToString();
        var newValue = ((TextBox)e.EditingElement).Text;
        var id = editedMachine.ID;
            
        if (newValue == null)
            return;

        switch (columnName)
        {
            case "Registračné číslo":
                editedMachine.ID = newValue;
                break;
            case "Názov":
                editedMachine.Name = newValue;
                break;
            case "Dátum revízie":
                editedMachine.LastRevisionDate = DateTime.Parse(newValue);
                editedMachine.RevisionValidity = 
                    editedMachine.LastRevisionDate.AddMonths(editedMachine.RevisionInterval);
                break;
            case "Interval revízie":
                editedMachine.RevisionInterval = int.Parse(newValue);
                editedMachine.RevisionValidity =
                    editedMachine.LastRevisionDate.AddMonths(editedMachine.RevisionInterval);
                break;
            case "Číslo protokolu":
                editedMachine.Protocol = newValue;
                break;
            case "Typ":
                editedMachine.Type = newValue;
                break;
            case "Výrobné číslo":
                editedMachine.ManufacturingNumber = newValue;
                break;
            case "Výrobca":
                editedMachine.Manufacturer = newValue;
                break;
            case "Lokácia":
                editedMachine.Location = newValue;
                break;
            case "Vlastník":
                editedMachine.Owner = newValue;
                break;
            case "Poznámka":
                editedMachine.Note = newValue;
                break;
        }

        await ((MainViewModel)DataContext).UpdateMachineAsync(editedMachine, id, 0);
    }
    
    private async void UpdateBig(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit) return;
        if (e.Row.DataContext is not BigMachine editedMachine)
            return;
            
        var columnName = e.Column.Header.ToString();
        var newValue = ((TextBox)e.EditingElement).Text;
        var id = editedMachine.ID;
            
        if (newValue == null)
            return;

        switch (columnName)
        {
            case "Registračné číslo":
                editedMachine.ID = newValue;
                break;
            case "Názov":
                editedMachine.Name = newValue;
                break;
            case "Dátum revízie":
                editedMachine.LastRevisionDate = DateTime.Parse(newValue);
                editedMachine.RevisionValidity = 
                    editedMachine.LastRevisionDate.AddMonths(editedMachine.RevisionInterval);
                break;
            case "Interval revízie":
                editedMachine.RevisionInterval = int.Parse(newValue);
                editedMachine.RevisionValidity =
                    editedMachine.LastRevisionDate.AddMonths(editedMachine.RevisionInterval);
                break;
            case "Číslo protokolu":
                editedMachine.Protocol = newValue;
                break;
            case "Typ":
                editedMachine.Type = newValue;
                break;
            case "Výrobné číslo":
                editedMachine.ManufacturingNumber = newValue;
                break;
            case "Výrobca":
                editedMachine.Manufacturer = newValue;
                break;
            case "Lokácia":
                editedMachine.Location = newValue;
                break;
            case "Vlastník":
                editedMachine.Owner = newValue;
                break;
            case "Poznámka":
                editedMachine.Note = newValue;
                break;
        }

        await ((MainViewModel)DataContext).UpdateMachineAsync(editedMachine, id, 1);
    }
    
    private async void ChooseLocation(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFolderDialog
        {
            Title = "Choose Location",
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        var selectedDirectory = await openFolderDialog.ShowAsync(this);

        if (!string.IsNullOrWhiteSpace(selectedDirectory))
        {
            (DataContext as MainViewModel)?.ExcelCreateAsync(selectedDirectory);
            Console.WriteLine($"Selected Directory: {selectedDirectory}");
        }
    }
    
    private void DeleteSmallMachine_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SmallMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.Delete(0, machineId);
    }

    private void DeleteBigMachine_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: BigMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.Delete(1, machineId);
    }
    
    private void TempDisableSmall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SmallMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(0, machineId, 1);
    }
    
    private void TempDisableBig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: BigMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(1, machineId, 1);
    }
    
    private void DisableSmall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SmallMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(0, machineId, 2);
    }
    
    private void DisableBig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: BigMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(1, machineId, 2);
    }

    private void BackActiveSmall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SmallMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(0, machineId, 0);
    }
    
    private void BackActiveBig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: BigMachine machine }) return;
        var machineId = machine.ID;
        (DataContext as MainViewModel)?.StatusChange(1, machineId, 0);
    }
    
    private void Logout(object sender, RoutedEventArgs e)
    {   
        (Application.Current as App)?.OpenLoginWindow();
        (Application.Current as App)?.CloseMainWindow();
    }
    
    private void CreateUser(object sender, RoutedEventArgs e)
    {
        var createUserWindow = new CreateUserWindow(_dataService);
        createUserWindow.Show();
    }
}