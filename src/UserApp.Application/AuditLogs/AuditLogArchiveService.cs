using UserApp.Application.AuditLogs.Interfaces;
using UserApp.Domain.AuditLogs;
using UserApp.Domain.Common;

namespace UserApp.Application.AuditLogs;

public class AuditLogArchiveService : IAuditLogArchiveService
{
    private readonly IAuditLogRepository _auditLogRepo;
    private readonly IAuditLogArchiveRepository _archiveRepo;
    private readonly IBaseRepository<AuditLog> _auditLogBaseRepo;

    private const int BatchSize = 1000;

    public AuditLogArchiveService(
        IAuditLogRepository auditLogRepo,
        IAuditLogArchiveRepository archiveRepo,
        IBaseRepository<AuditLog> auditLogBaseRepo)
    {
        _auditLogRepo = auditLogRepo;
        _archiveRepo = archiveRepo;
        _auditLogBaseRepo = auditLogBaseRepo;
    }

    public async Task ArchiveYesterdayAsync(CancellationToken cancellationToken = default)
    {
        var yesterdayStart = TimeHelper.Now.Date.AddDays(-1);
        var yesterdayEnd = yesterdayStart.AddDays(1);

        if (await _archiveRepo.HasArchivedDataForDateAsync(yesterdayStart))
        {
            var existingCount = await _archiveRepo.CountArchivedForDateAsync(yesterdayStart);
            var sourceCount = await _auditLogRepo.CountForDateRangeAsync(yesterdayStart, yesterdayEnd);

            if (existingCount >= sourceCount)
                return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _auditLogRepo.GetBatchForDateRangeAsync(
                yesterdayStart, yesterdayEnd, 0, BatchSize);

            if (batch.Count == 0)
                break;

            var archives = batch.Select(a => new AuditLogArchive
            {
                UserName = a.UserName,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                PageName = a.PageName,
                FunctionName = a.FunctionName,
                AffectedColumns = a.AffectedColumns,
                OldValues = a.OldValues,
                NewValues = a.NewValues,
                ArchivedAt = TimeHelper.Now
            }).ToList();

            foreach (var archive in archives)
            {
                await _archiveRepo.AddAsync(archive);
            }

            await _archiveRepo.SaveChangesAsync();

            foreach (var log in batch)
            {
                _auditLogBaseRepo.Remove(log);
            }

            await _auditLogBaseRepo.SaveChangesAsync();
        }
    }
}
