using Microsoft.Extensions.Logging;
using Quartz;
using UserApp.Application.AuditLogs.Interfaces;

namespace UserApp.Web.Jobs;

[DisallowConcurrentExecution]
public class AuditLogArchiveJob : IJob
{
    private readonly IAuditLogArchiveService _archiveService;
    private readonly ILogger<AuditLogArchiveJob> _logger;

    public AuditLogArchiveJob(IAuditLogArchiveService archiveService, ILogger<AuditLogArchiveJob> logger)
    {
        _archiveService = archiveService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("AuditLogArchiveJob triggered at {Time}", DateTimeOffset.Now);
        await _archiveService.ArchiveYesterdayAsync(context.CancellationToken);
        _logger.LogInformation("AuditLogArchiveJob finished at {Time}", DateTimeOffset.Now);
    }
}
