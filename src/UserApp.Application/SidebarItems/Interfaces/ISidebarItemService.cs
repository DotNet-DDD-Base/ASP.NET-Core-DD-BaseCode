using UserApp.Application.Common;
using UserApp.Domain.SidebarItems;

namespace UserApp.Application.SidebarItems.Interfaces;

public interface ISidebarItemService : IBaseService<SidebarItem>
{
    Task<List<SidebarItem>> GetActiveAsync();
}
