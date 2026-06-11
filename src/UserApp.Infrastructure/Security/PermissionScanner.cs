using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace UserApp.Infrastructure.Security;

public static class PermissionScanner
{
    public static List<string> GetAllPermissions(Assembly assembly)
    {
        var permissions = new List<string>();

        var controllers = assembly.GetTypes()
            .Where(t =>
                typeof(Controller).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.Name.EndsWith("Controller"));

        foreach (var controller in controllers)
        {
            var controllerName = controller.Name.Replace("Controller", "");

            var actions = GetActionMethods(controller);

            foreach (var action in actions)
            {
                permissions.Add($"{controllerName}.{action}");
            }
        }

        return permissions.Distinct().OrderBy(x => x).ToList();
    }

    private static List<string> GetActionMethods(Type controller)
    {
        var baseControllerType = typeof(Controller);

        return controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name)
            .Distinct()
            .ToList();
    }
}