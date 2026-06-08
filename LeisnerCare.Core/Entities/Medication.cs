namespace LeisnerCare.Core.Entities;

public class Medication
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty; // "Morgen, Aften" eller "Hver 4. time"
    public TimeSpan? TimeOfDay { get; set; } // F.eks. 08:00
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Patient? Patient { get; set; }
    public List<MedicationLog> Logs { get; set; } = new();
}