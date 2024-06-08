using System;
using Avalonia.Media;
using SQLite;

namespace MachineEvidency.Data;

public class Machine
{
    [PrimaryKey]
    public string ID { get; set; }
    public string Name { get; set; }
    public DateTime LastRevisionDate { get; set; }
    public int RevisionInterval { get; set; }
    public string Protocol { get; set; }
    public string Type { get; set; }
    public string ManufacturingNumber { get; set; }
    public string Manufacturer { get; set; }
    public string Location { get; set; }
    public string Owner { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Note { get; set; }
    public DateTime RevisionValidity { get; set; }
    public string Color { get; set; }
    public int Status { get; set; } // 0 aktívne 1 dočasne vyradené 2 vyradené
}