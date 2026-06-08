using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;

namespace LeisnerCare.Application.Services;

public class PatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Patient>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Patient?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<Patient> CreateAsync(Patient patient) => _repository.AddAsync(patient);
    public Task UpdateAsync(Patient patient) => _repository.UpdateAsync(patient);
    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
}