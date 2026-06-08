using LeisnerCare.Core.Entities;

namespace LeisnerCare.Core.Interfaces;

public interface IMedicationRepository
{
    Task<List<Medication>> GetByPatientIdAsync(int patientId);
    Task<Medication?> GetByIdAsync(int id);
    Task<Medication> AddAsync(Medication medication);
    Task<MedicationLog> AddLogAsync(MedicationLog log);
}