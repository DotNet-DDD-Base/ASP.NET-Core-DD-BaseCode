namespace UserApp.Application.AuditLogs.Interfaces;

public interface IAuditLogArchiveService
{
    Task ArchiveYesterdayAsync(CancellationToken cancellationToken = default);
}
