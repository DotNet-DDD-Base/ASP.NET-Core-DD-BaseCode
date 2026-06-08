using UserApp.Domain.{{Name}}s;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class {{Name}}Repository : BaseRepository<{{Name}}>, I{{Name}}Repository
{
    public {{Name}}Repository(AppDbContext db) : base(db)
    {
    }
}
