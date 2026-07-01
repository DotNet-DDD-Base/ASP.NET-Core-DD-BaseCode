using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApp.Domain.AuditLogs;

namespace UserApp.Infrastructure.Persistence.Configurations;

public class AuditLogArchiveConfiguration : IEntityTypeConfiguration<AuditLogArchive>
{
    public void Configure(EntityTypeBuilder<AuditLogArchive> b)
    {
        b.ToTable("AuditLogsArchive");

        b.HasKey(x => x.Id);

        b.Property(x => x.UserName).HasMaxLength(200).IsRequired();
        b.Property(x => x.Action).HasMaxLength(50).IsRequired();
        b.Property(x => x.EntityName).HasMaxLength(200).IsRequired();
        b.Property(x => x.EntityId).HasMaxLength(100);
        b.Property(x => x.PageName).HasMaxLength(200);
        b.Property(x => x.FunctionName).HasMaxLength(200);
        b.Property(x => x.ArchivedAt).IsRequired();

        b.HasIndex(x => x.ArchivedAt);
        b.HasIndex(x => x.CreatedAt);
    }
}
