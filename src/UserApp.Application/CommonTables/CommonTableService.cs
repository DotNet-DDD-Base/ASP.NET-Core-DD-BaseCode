using UserApp.Application.Common;
using UserApp.Domain.CommonTables;
using UserApp.Application.CommonTables.Interfaces;

namespace UserApp.Application.CommonTables;

public class CommonTableService : BaseService<CommonTable>, ICommonTableService
{
    public CommonTableService(ICommonTableRepository repo) : base(repo)
    {
    }
}
