using UserApp.Domain.Tables;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class TableRepository : BaseRepository<Table>, ITableRepository
{
    public TableRepository(AppDbContext db) : base(db)
    {
    }
}
