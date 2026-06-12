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
            if (controllerName.EndsWith("Api"))
                controllerName = controllerName[..^3];

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
        var methods = new HashSet<string>();
        var type = controller;

        while (type != null && type != typeof(Controller) && type != typeof(ControllerBase) && type != typeof(object))
        {
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!m.IsSpecialName && !m.Name.Contains('<'))
                    methods.Add(m.Name);
            }
            type = type.BaseType;
        }

        return methods.ToList();
    }
}