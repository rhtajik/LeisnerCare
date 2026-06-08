using LeisnerCare.Core.Entities;

namespace LeisnerCare.Core.Interfaces;

public interface IObservationRepository
{
    Task<List<Observation>> GetByPatientIdAsync(int patientId);
    Task<List<Observation>> GetAllAsync();
    Task<Observation> AddAsync(Observation observation);
}