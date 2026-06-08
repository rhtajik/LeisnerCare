using LeisnerCare.Core.Entities;

namespace LeisnerCare.Core.Interfaces;

public interface ISymptomRepository
{
    Task<List<Symptom>> GetByPatientIdAsync(int patientId);
    Task<List<Symptom>> GetAllAsync();
    Task<Symptom> AddAsync(Symptom symptom);
}