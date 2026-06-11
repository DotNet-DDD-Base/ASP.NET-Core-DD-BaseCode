namespace UserApp.Application.Common.Interfaces;

public interface IPermissionChecker
{
    Task<bool> HasPermissionAsync(Guid userId, string permission);

    Task<HashSet<string>> GetUserPermissionsAsync(Guid userId);
}