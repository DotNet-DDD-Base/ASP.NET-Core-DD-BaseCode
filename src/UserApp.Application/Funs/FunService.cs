using UserApp.Application.Common;
using UserApp.Domain.Funs;
using UserApp.Application.Funs.Interfaces;

namespace UserApp.Application.Funs;

public class FunService : BaseService<Fun>, IFunService
{
    public FunService(IFunRepository repo) : base(repo)
    {
    }
}
