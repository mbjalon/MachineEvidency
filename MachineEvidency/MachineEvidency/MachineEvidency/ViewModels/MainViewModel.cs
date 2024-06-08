using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MachineEvidency.AuthService;
using MachineEvidency.Data;
using OfficeOpenXml;
using ReactiveUI;

namespace MachineEvidency.ViewModels;

public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly DataService _databaseService;
    private List<Machine>? _smallMachines;
    private List<Machine>? _tempDisabledSmallMachines;
    private List<Machine>? _disabledSmallMachines;
    private List<Machine>? _bigMachines;
    private List<Machine>? _tempDisabledBigMachines;
    private List<Machine>? _disabledBigMachines;
    private string _searchText;
    private User _loggedInUser;
    private string _errorMessage;

    public ICommand SearchCommand { get; }
    public bool IsAdmin => LoggedInUser.Role == UserRole.Admin;
    public new event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel(DataService dataService)
    {
        _databaseService = dataService;
        _ = LoadDataAsync();
        SearchCommand = new AsyncRelayCommand(Search);
    }

    public List<Machine>? SmallMachines
    {
        get => _smallMachines;
        set
        {
            _smallMachines = value;
            OnPropertyChanged();
        }
    }
    
    public List<Machine>? TempDisabledSmallMachines
    {
        get => _tempDisabledSmallMachines;
        set
        {
            _tempDisabledSmallMachines = value;
            OnPropertyChanged();
        }
    }
    public List<Machine>? DisabledSmallMachines
    {
        get => _disabledSmallMachines;
        set
        {
            _disabledSmallMachines = value;
            OnPropertyChanged();
        }
    }

    public List<Machine>? BigMachines
    {
        get => _bigMachines;
        set
        {
            _bigMachines = value;
            OnPropertyChanged();
        }
    }
    
    public List<Machine>? TempDisabledBigMachines
    {
        get => _tempDisabledBigMachines;
        set
        {
            _tempDisabledBigMachines = value;
            OnPropertyChanged();
        }
    }
    public List<Machine>? DisabledBigMachines
    {
        get => _disabledBigMachines;
        set
        {
            _disabledBigMachines = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public User LoggedInUser
    {
        get => _loggedInUser;
        set => this.RaiseAndSetIfChanged(ref _loggedInUser, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    
    public async Task LoadDataAsync()
    {
        SmallMachines = await _databaseService.GetAllMachinesAsync(0, 0);
        UpdateEditCellForeground(_smallMachines);
        BigMachines = await _databaseService.GetAllMachinesAsync(1, 0);
        UpdateEditCellForeground(_bigMachines);
        TempDisabledSmallMachines = await _databaseService.GetAllMachinesAsync(0, 1);
        UpdateEditCellForeground(_tempDisabledSmallMachines);
        TempDisabledBigMachines = await _databaseService.GetAllMachinesAsync(1, 1);
        UpdateEditCellForeground(_tempDisabledBigMachines);
        DisabledSmallMachines = await _databaseService.GetAllMachinesAsync(0, 2);
        UpdateEditCellForeground(_disabledSmallMachines);
        DisabledBigMachines = await _databaseService.GetAllMachinesAsync(1, 2);
        UpdateEditCellForeground(_disabledBigMachines);
    }

    public async Task UpdateMachineAsync(Machine machine, string id, int type)
    {
        if (_loggedInUser.Role == UserRole.User)
        {
            ErrorMessage = "Nedostatočné oprávnenie";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            Console.WriteLine("Nedostatočné oprávnenie");
            return;
        }
        
        try
        {
            await _databaseService.UpdateMachineAsync(machine, id, type);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating machine: {ex.Message}");
        }
    }

    public async Task Delete(int type, string id)
    {
        if (_loggedInUser.Role == UserRole.Admin)
        {
            await _databaseService.DeleteAsync(type, id);
            await LoadDataAsync();
            return;
        }
        
        ErrorMessage = "Nedostatočné oprávnenie";
        
        Console.WriteLine("Nedostatočné oprávnenie");
    }

    public async Task StatusChange(int type, string id, int status)
    {
        if (_loggedInUser.Role == UserRole.User)
        {
            ErrorMessage = "Nedostatočné oprávnenie";
            Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(_ => ErrorMessage = "");  
            Console.WriteLine("Nedostatočné oprávnenie");
            return;
        }
        await _databaseService.StatusChangeAsync(type, id, status);
        await LoadDataAsync();
    }

    public async void ExcelCreateAsync(string filePath)
    {
        try
        {
            var machineLists = new Dictionary<string, List<Machine>>
            {
                { "Malé zariadenia", _smallMachines ?? throw new InvalidOperationException() },
                { "Dočasne vyradené malé", _tempDisabledSmallMachines ?? throw new InvalidOperationException() },
                { "Vyradené malé", _disabledSmallMachines ?? throw new InvalidOperationException() },
                { "Veľké zariadenia", _bigMachines ?? throw new InvalidOperationException() },
                { "Dočasne vyradené veľké", _tempDisabledBigMachines ?? throw new InvalidOperationException() },
                { "Vyradené veľké", _disabledBigMachines ?? throw new InvalidOperationException() }
            };
            
            await CreateExcelFileAsync(filePath, machineLists);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadDataAsync();
        }
        else
        {
            var filteredSmallMachines = FilterMachines(_smallMachines);
            var filteredTempDisabledSmallMachines = FilterMachines(_tempDisabledSmallMachines);
            var filteredDisabledSmallMachines = FilterMachines(_disabledSmallMachines);

            var filteredBigMachines = FilterMachines(_bigMachines);
            var filteredTempDisabledBigMachines = FilterMachines(_tempDisabledBigMachines);
            var filteredDisabledBigMachines = FilterMachines(_disabledBigMachines);

            SmallMachines = filteredSmallMachines;
            TempDisabledSmallMachines = filteredTempDisabledSmallMachines;
            DisabledSmallMachines = filteredDisabledSmallMachines;

            BigMachines = filteredBigMachines;
            TempDisabledBigMachines = filteredTempDisabledBigMachines;
            DisabledBigMachines = filteredDisabledBigMachines;
        }
    }
    
    private async Task CreateExcelFileAsync(string filePath, Dictionary<string, List<Machine>> machineLists)
    {
        using var excelPackage = new ExcelPackage();
        foreach (var kvp in machineLists)
        {
            var worksheet = excelPackage.Workbook.Worksheets.Add(kvp.Key);

            string[] columnHeaders = ["ID", "Name", "Protocol", "Type", "Manufacturing Number", "Manufacturer", "Location", "Owner", "Note", "Revision Interval", "Last Revision Date", "Registration Date", "Revision Validity"
            ];
            for (var i = 0; i < columnHeaders.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = columnHeaders[i];
            }

            for (var i = 0; i < kvp.Value.Count; i++)
            {
                var machine = kvp.Value[i];
                worksheet.Cells[i + 2, 1].Value = machine.ID;
                worksheet.Cells[i + 2, 2].Value = machine.Name;
                worksheet.Cells[i + 2, 3].Value = machine.Protocol;
                worksheet.Cells[i + 2, 4].Value = machine.Type;
                worksheet.Cells[i + 2, 5].Value = machine.ManufacturingNumber;
                worksheet.Cells[i + 2, 6].Value = machine.Manufacturer;
                worksheet.Cells[i + 2, 7].Value = machine.Location;
                worksheet.Cells[i + 2, 8].Value = machine.Owner;
                worksheet.Cells[i + 2, 9].Value = machine.Note;
                worksheet.Cells[i + 2, 10].Value = machine.RevisionInterval;
                worksheet.Cells[i + 2, 11].Value = machine.LastRevisionDate.ToString("yyyy-MM-dd"); 
                worksheet.Cells[i + 2, 12].Value = machine.RegistrationDate.ToString("yyyy-MM-dd"); 
                worksheet.Cells[i + 2, 13].Value = machine.RevisionValidity.ToString("yyyy-MM-dd");
            }
        }

        await Task.Run(() => excelPackage.SaveAs(new FileInfo(filePath + $"/Evidencia{DateTime.Today}.xlsx")));
    }
    
    private void UpdateEditCellForeground(IReadOnlyCollection<Machine>? machines)
    {
        if (machines == null) return;
        foreach (var machine in machines)
        {
            if (machine.RevisionValidity < DateTime.Today)
            {
                machine.Color = "#cc1d1d";
            }
            else if (machine.RevisionValidity < DateTime.Today.AddMonths(1))
            {
                machine.Color = "#e8c113";
            }
            else
            {
                machine.Color = "#ffffff";
            }
        }
    }
    
    private List<Machine> FilterMachines(IEnumerable<Machine>? machines)
    {
        if (machines == null) return [];

        return machines.Where(machine =>
            (machine.ID ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Name ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Protocol ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Type ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.ManufacturingNumber ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Manufacturer ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Location ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Owner ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (machine.Note ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            machine.RevisionInterval.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            machine.LastRevisionDate.Date.ToString(CultureInfo.CurrentCulture).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            machine.RegistrationDate.Date.ToString(CultureInfo.InvariantCulture).Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            machine.RevisionValidity.Date.ToString(CultureInfo.InvariantCulture).Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}