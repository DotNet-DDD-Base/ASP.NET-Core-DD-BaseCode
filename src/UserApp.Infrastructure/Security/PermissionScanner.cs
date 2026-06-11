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

            var actions = GetCrudActions(controller);

            foreach (var action in actions)
            {
                permissions.Add($"{controllerName}.{action}");
            }
        }

        return permissions.Distinct().OrderBy(x => x).ToList();
    }

    private static List<string> GetCrudActions(Type controller)
    {
        var actions = new HashSet<string>();

        var methods = controller.GetMethods(
            BindingFlags.Public |
            BindingFlags.Instance);

        foreach (var method in methods)
        {
            if (method.IsSpecialName)
                continue;

            if (method.DeclaringType == typeof(object))
                continue;

            switch (method.Name)
            {
                case "Index":
                case "Details":
                case "Create":
                case "Edit":
                case "Delete":
                case "DeleteConfirmed":
                    actions.Add(method.Name);
                    break;
            }
        }

        return actions.ToList();
    }
}