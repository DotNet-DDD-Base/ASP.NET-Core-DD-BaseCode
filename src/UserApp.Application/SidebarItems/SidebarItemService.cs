using UserApp.Application.Common;
using UserApp.Domain.SidebarItems;
using UserApp.Application.SidebarItems.Interfaces;

namespace UserApp.Application.SidebarItems;

public class SidebarItemService : BaseService<SidebarItem>, ISidebarItemService
{
    private readonly ISidebarItemRepository _sidebarRepo;

    public SidebarItemService(ISidebarItemRepository repo) : base(repo)
    {
        _sidebarRepo = repo;
    }

    public Task<List<SidebarItem>> GetActiveAsync()
    {
        return _sidebarRepo.GetActiveAsync();
    }
}
