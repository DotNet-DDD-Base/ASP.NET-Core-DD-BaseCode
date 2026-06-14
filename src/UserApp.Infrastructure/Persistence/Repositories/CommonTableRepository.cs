using UserApp.Domain.CommonTables;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class CommonTableRepository : BaseRepository<CommonTable>, ICommonTableRepository
{
    public CommonTableRepository(AppDbContext db) : base(db)
    {
    }
}
