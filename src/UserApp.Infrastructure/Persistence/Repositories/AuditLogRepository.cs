using Microsoft.EntityFrameworkCore;
using UserApp.Domain.AuditLogs;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext db) : base(db) { }

    public override async Task<List<AuditLog>> ListAsync(int skip, int take)
    {
        return await _set.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).ToListAsync();
    }

    public async Task<List<AuditLog>> SearchAsync(string searchTerm, int skip, int take)
    {
        var q = _set.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            q = q.Where(x => x.UserName.ToLower().Contains(term)
                          || x.Action.ToLower().Contains(term)
                          || x.EntityName.ToLower().Contains(term)
                          || x.PageName.ToLower().Contains(term)
                          || x.EntityId.ToLower().Contains(term));
        }
        return await q.OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take).ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        var q = _set.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            q = q.Where(x => x.UserName.ToLower().Contains(term)
                          || x.Action.ToLower().Contains(term)
                          || x.EntityName.ToLower().Contains(term)
                          || x.PageName.ToLower().Contains(term)
                          || x.EntityId.ToLower().Contains(term));
        }
        return await q.CountAsync();
    }

    public async Task<List<AuditLog>> GetBatchForDateRangeAsync(DateTime from, DateTime to, int skip, int take)
    {
        return await _set
            .Where(x => x.CreatedAt >= from && x.CreatedAt < to)
            .OrderBy(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountForDateRangeAsync(DateTime from, DateTime to)
    {
        return await _set
            .CountAsync(x => x.CreatedAt >= from && x.CreatedAt < to);
    }
}
