using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;

namespace LeisnerCare.Application.Services;

public class SymptomService
{
    private readonly ISymptomRepository _repository;

    public SymptomService(ISymptomRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Symptom>> GetPatientHistoryAsync(int patientId) =>
        _repository.GetByPatientIdAsync(patientId);

    public Task<List<Symptom>> GetAllAsync() =>
        _repository.GetAllAsync();

    public Task<Symptom> CreateAsync(Symptom symptom) =>
        _repository.AddAsync(symptom);
}