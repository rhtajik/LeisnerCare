using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;

namespace LeisnerCare.Application.Services;

public class MedicationService
{
    private readonly IMedicationRepository _repository;

    public MedicationService(IMedicationRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Medication>> GetPatientMedicationsAsync(int patientId) =>
        _repository.GetByPatientIdAsync(patientId);

    public Task<Medication?> GetByIdAsync(int id) =>
        _repository.GetByIdAsync(id);

    public Task<Medication> CreateAsync(Medication medication) =>
        _repository.AddAsync(medication);

    public Task<MedicationLog> LogMedicationAsync(MedicationLog log) =>
        _repository.AddLogAsync(log);
}