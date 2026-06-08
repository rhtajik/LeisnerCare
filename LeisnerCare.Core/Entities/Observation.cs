namespace LeisnerCare.Core.Entities;

public class Observation
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty; // Patient, Relative, Staff
    public string Content { get; set; } = string.Empty;
    public bool IsClinical { get; set; } = false; // Kun Staff kan markere
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Patient? Patient { get; set; }
}