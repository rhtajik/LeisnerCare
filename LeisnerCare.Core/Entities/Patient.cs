namespace LeisnerCare.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string CprNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string? ContactPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // GDPR Samtykke
        public bool ConsentGiven { get; set; } = false;
        public DateTime? ConsentDate { get; set; }

        // Pårørende tilknytning
        public string? RelativeUserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}