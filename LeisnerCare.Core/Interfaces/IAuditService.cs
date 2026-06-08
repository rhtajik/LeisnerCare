using LeisnerCare.Core.Entities;

namespace LeisnerCare.Core.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string entityType,
        int entityId,
        string userId,
        string userName,
        int? patientId = null,
        string? details = null);

    Task<List<AuditLog>> GetAllAsync(); 
}