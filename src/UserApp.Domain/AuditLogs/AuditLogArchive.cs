using UserApp.Domain.Common;

namespace UserApp.Domain.AuditLogs;

public class AuditLogArchive : Entity<Guid>
{
    public AuditLogArchive()
    {
        Id = Guid.NewGuid();
    }

    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? AffectedColumns { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime ArchivedAt { get; set; }
}
