using UserApp.Domain.Common;

namespace UserApp.Domain.SidebarItems;

public interface ISidebarItemRepository : IBaseRepository<SidebarItem>
{
    Task<List<SidebarItem>> GetActiveAsync();
}
