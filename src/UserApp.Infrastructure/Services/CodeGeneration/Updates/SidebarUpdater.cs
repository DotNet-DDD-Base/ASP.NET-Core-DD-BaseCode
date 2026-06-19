using System;
using Microsoft.AspNetCore.Routing;
using UserApp.Application.Common;
using UserApp.Domain.SidebarItems;

namespace UserApp.Infrastructure.Services.CodeGeneration.Updates;

public class SidebarUpdater
{
    public void Add(string moduleName, Guid groupId)
    {
        var repo = ServiceProviderAccessor.Current?.GetService(typeof(ISidebarItemRepository)) as ISidebarItemRepository;
        if (repo == null) return;

        var existing = repo.GetActiveAsync().GetAwaiter().GetResult();
        if (existing.Any(x => x.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.GroupId == groupId))
            return;

        var item = new SidebarItem
        {
            ModuleName = moduleName,
            ControllerName = moduleName,
            GroupId = groupId,
            DisplayOrder = 0,
            IsActive = true
        };

        repo.AddAsync(item).GetAwaiter().GetResult();
        repo.SaveChangesAsync().GetAwaiter().GetResult();
    }
}
