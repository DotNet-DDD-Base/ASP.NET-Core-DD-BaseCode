using UserApp.Application.Common;
using UserApp.Domain.Tables;
using UserApp.Application.Tables.Interfaces;

namespace UserApp.Application.Tables;

public class TableService : BaseService<Table>, ITableService
{
    public TableService(ITableRepository repo) : base(repo)
    {
    }
}
