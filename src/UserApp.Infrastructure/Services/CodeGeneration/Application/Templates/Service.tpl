using UserApp.Application.Common;
using UserApp.Domain.{{Name}}s;
using UserApp.Application.{{Name}}s.Interfaces;

namespace UserApp.Application.{{Name}}s;

public class {{Name}}Service : BaseService<{{Name}}>, I{{Name}}Service
{
    public {{Name}}Service(I{{Name}}Repository repo) : base(repo)
    {
    }
}
