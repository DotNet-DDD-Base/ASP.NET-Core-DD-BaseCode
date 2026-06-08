using UserApp.Domain.Funs;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class FunRepository : BaseRepository<Fun>, IFunRepository
{
    public FunRepository(AppDbContext db) : base(db)
    {
    }
}
