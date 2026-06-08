namespace LeisnerCare.Core.Entities;

public class MedicationLog
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    public int? Effectiveness { get; set; } // 1-5 skala
    public string? Note { get; set; }

    public Medication? Medication { get; set; }
}