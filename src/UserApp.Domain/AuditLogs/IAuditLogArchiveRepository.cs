using UserApp.Domain.Common;

namespace UserApp.Domain.AuditLogs;

public interface IAuditLogArchiveRepository : IBaseRepository<AuditLogArchive>
{
    Task<bool> HasArchivedDataForDateAsync(DateTime date);
    Task<int> CountArchivedForDateAsync(DateTime date);
    Task<List<AuditLogArchive>> ListForDateRangeAsync(DateTime from, DateTime to, int skip, int take);
    Task<int> CountForDateRangeAsync(DateTime from, DateTime to);
    Task<List<AuditLogArchive>> SearchForDateRangeAsync(string searchTerm, DateTime from, DateTime to, int skip, int take);
    Task<int> CountSearchForDateRangeAsync(string searchTerm, DateTime from, DateTime to);
}
