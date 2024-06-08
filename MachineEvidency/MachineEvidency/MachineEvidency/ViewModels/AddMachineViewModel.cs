using System;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MachineEvidency.Data;
using ReactiveUI;

namespace MachineEvidency.ViewModels;

public class AddMachineViewModel : ViewModelBase
{ 
    private readonly DataService _dataService;
    private string _errorMessage;
    private string _successMessage;
    private int _machineType;
    private string _id;
    private string _name;
    private DateTime _lastRevisionDate;
    private int _revisionInterval;
    private string _protocol;
    private string _type;
    private string _manufacturingNumber;
    private string _manufacturer;
    private string _location;
    private string _owner;
    private string _note;
    
    public AddMachineViewModel(DataService dataService)
    { 
        _dataService = dataService;
        AddCommand = new RelayCommand(Save);
    }

    public int MachineType
    {
        get => _machineType;
        set => this.RaiseAndSetIfChanged(ref _machineType, value);
    }
    
    public string ID
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public DateTime LastRevisionDate
    {
        get => _lastRevisionDate;
        set => this.RaiseAndSetIfChanged(ref _lastRevisionDate, value);
    }

    public int RevisionInterval
    {
        get => _revisionInterval;
        set => this.RaiseAndSetIfChanged(ref _revisionInterval, value);
    }

    public string Protocol
    {
        get => _protocol;
        set => this.RaiseAndSetIfChanged(ref _protocol, value);
    }

    public string Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    public string ManufacturingNumber
    {
        get => _manufacturingNumber;
        set => this.RaiseAndSetIfChanged(ref _manufacturingNumber, value);
    }

    public string Manufacturer
    {
        get => _manufacturer;
        set => this.RaiseAndSetIfChanged(ref _manufacturer, value);
    }

    public string Location
    {
        get => _location;
        set => this.RaiseAndSetIfChanged(ref _location, value);
    }
    
    public string Owner
    {
        get => _owner;
        set => this.RaiseAndSetIfChanged(ref _owner, value);
    }

    public string Note
    {
        get => _note;
        set => this.RaiseAndSetIfChanged(ref _note, value);
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
    
    public ICommand AddCommand { get; }

    private bool Validator(Machine machine)
    {
        if (string.IsNullOrWhiteSpace(machine.ID))
        {
            ErrorMessage = "Zadaj registračné číslo";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            return false;
        }

        if (string.IsNullOrWhiteSpace(machine.Name))
        {
            ErrorMessage = "Zadaj názov";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            return false;
        }

        if (machine.RevisionInterval < 1)
        {
            ErrorMessage = "Zadaj platný interval revízie";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            return false;
        }

        if (machine.LastRevisionDate > DateTime.Today)
        {
            ErrorMessage = "Zadaj platný dátum revízie";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            return false;
        }

        return true;
    }

    private async void Save()
    {
        Machine machine;
        
        if (_machineType == 0)
        {
            machine = new SmallMachine
            {
                ID = _id,
                Name = _name,
                LastRevisionDate = _lastRevisionDate,
                RevisionInterval = _revisionInterval,
                Protocol = _protocol,
                Type = _type,
                ManufacturingNumber = _manufacturingNumber,
                Manufacturer = _manufacturer,
                Location = _location,
                Owner = _owner,
                RegistrationDate = DateTime.Today,
                Note = _note,
                RevisionValidity = _lastRevisionDate.AddMonths(_revisionInterval),
                Status = 0
            };
        }
        else
        {
            machine = new BigMachine
            {
                ID = _id,
                Name = _name,
                LastRevisionDate = _lastRevisionDate,
                RevisionInterval = _revisionInterval,
                Protocol = _protocol,
                Type = _type,
                ManufacturingNumber = _manufacturingNumber,
                Manufacturer = _manufacturer,
                Location = _location,
                Owner = _owner,
                RegistrationDate = DateTime.Today,
                Note = _note,
                RevisionValidity = _lastRevisionDate.AddMonths(_revisionInterval),
                Status = 0
            };
        }

        if (Validator(machine))
        {
            try
            {

                if (await _dataService.AddMachineAsync(machine, _machineType))
                {
                    SuccessMessage = "Úspešne zaregistrované";
                    Observable.Timer(TimeSpan.FromSeconds(10))
                        .Subscribe(_ => SuccessMessage = "");
                }
                else
                {
                    ErrorMessage = "Registrácia neúspešná";
                    Observable.Timer(TimeSpan.FromSeconds(10))
                        .Subscribe(_ => SuccessMessage = "");
                }
            }
            catch (Exception e)
            {
                ErrorMessage = "Chybné údaje";
                Observable.Timer(TimeSpan.FromSeconds(10))
                    .Subscribe(_ => ErrorMessage = "");            
            }
        }
    }
}