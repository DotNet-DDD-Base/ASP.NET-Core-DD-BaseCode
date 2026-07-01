using UserApp.Domain.Common;

namespace UserApp.Domain.AuditLogs;

public interface IAuditLogRepository : IBaseRepository<AuditLog>
{
    Task<List<AuditLog>> SearchAsync(string searchTerm, int skip, int take);
    Task<int> CountSearchAsync(string searchTerm);
    Task<List<AuditLog>> GetBatchForDateRangeAsync(DateTime from, DateTime to, int skip, int take);
    Task<int> CountForDateRangeAsync(DateTime from, DateTime to);
}
