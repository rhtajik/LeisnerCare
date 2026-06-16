namespace LeisnerCare.Core.Entities;

public class Medication
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;

    // 3 tidspunkter 
    public TimeSpan? TimeOfDay { get; set; }   // F.eks. 08:00
    public TimeSpan? TimeOfDay2 { get; set; }  // F.eks. 14:00
    public TimeSpan? TimeOfDay3 { get; set; }  // F.eks. 20:00

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Patient? Patient { get; set; }
    public List<MedicationLog> Logs { get; set; } = new();
}