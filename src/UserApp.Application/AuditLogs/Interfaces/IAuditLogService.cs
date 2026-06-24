using UserApp.Application.Common;

namespace UserApp.Application.AuditLogs.Interfaces;

public interface IAuditLogService : IBaseService<Domain.AuditLogs.AuditLog>
{
    Task LogAsync(string userName, string action, string entityName, string entityId,
        string? pageName = null, string? functionName = null,
        string? affectedColumns = null, string? oldValues = null, string? newValues = null);
    Task<List<Domain.AuditLogs.AuditLog>> SearchAsync(string searchTerm, int skip, int take);
    Task<int> CountSearchAsync(string searchTerm);
}
