using Microsoft.EntityFrameworkCore;
using UserApp.Domain.AuditLogs;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class AuditLogArchiveRepository : BaseRepository<AuditLogArchive>, IAuditLogArchiveRepository
{
    public AuditLogArchiveRepository(AppDbContext db) : base(db) { }

    public async Task<bool> HasArchivedDataForDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await _set.AnyAsync(x => x.ArchivedAt >= start && x.ArchivedAt < end);
    }

    public async Task<int> CountArchivedForDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await _set.CountAsync(x => x.ArchivedAt >= start && x.ArchivedAt < end);
    }

    public async Task<List<AuditLogArchive>> ListForDateRangeAsync(DateTime from, DateTime to, int skip, int take)
    {
        return await _set
            .Where(x => x.CreatedAt >= from && x.CreatedAt < to)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountForDateRangeAsync(DateTime from, DateTime to)
    {
        return await _set
            .CountAsync(x => x.CreatedAt >= from && x.CreatedAt < to);
    }

    public async Task<List<AuditLogArchive>> SearchForDateRangeAsync(string searchTerm, DateTime from, DateTime to, int skip, int take)
    {
        var term = searchTerm.Trim().ToLower();
        return await _set
            .Where(x => x.CreatedAt >= from && x.CreatedAt < to
                && (x.UserName.ToLower().Contains(term)
                    || x.Action.ToLower().Contains(term)
                    || x.EntityName.ToLower().Contains(term)
                    || x.PageName.ToLower().Contains(term)
                    || x.EntityId.ToLower().Contains(term)))
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountSearchForDateRangeAsync(string searchTerm, DateTime from, DateTime to)
    {
        var term = searchTerm.Trim().ToLower();
        return await _set
            .CountAsync(x => x.CreatedAt >= from && x.CreatedAt < to
                && (x.UserName.ToLower().Contains(term)
                    || x.Action.ToLower().Contains(term)
                    || x.EntityName.ToLower().Contains(term)
                    || x.PageName.ToLower().Contains(term)
                    || x.EntityId.ToLower().Contains(term)));
    }
}
