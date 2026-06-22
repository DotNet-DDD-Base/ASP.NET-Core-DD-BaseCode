using System.Collections.Generic;
using System.IO;
using System.Text;
using UserApp.Application.Common.DTOs;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class ApplicationGenerator
{
    private readonly PathProvider _paths;
    private readonly FileManager _files;
    private readonly TemplateEngine _templates;

    public ApplicationGenerator(PathProvider paths, FileManager files, TemplateEngine templates)
    {
        _paths = paths;
        _files = files;
        _templates = templates;
    }

    public void Generate(string name, List<ModuleFieldDto> fields)
    {
        var applicationFolder = Path.Combine(_paths.SrcRoot, "UserApp.Application", $"{name}s");
        var interfacesFolder = Path.Combine(applicationFolder, "Interfaces");

        _files.EnsureDirectory(applicationFolder);
        _files.EnsureDirectory(interfacesFolder);

        var serviceContent = _templates.RenderFile(
            new[] { "Application", "Templates", "Service.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["ValidationCode"] = BuildValidationCode(name, fields)
            });

        var interfaceContent = _templates.RenderFile(
            new[] { "Application", "Templates", "Interface.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name
            });

        _files.WriteFile(Path.Combine(applicationFolder, $"{name}Service.cs"), serviceContent);
        _files.WriteFile(Path.Combine(interfacesFolder, $"I{name}Service.cs"), interfaceContent);
    }

    private static string BuildValidationCode(string name, List<ModuleFieldDto> fields)
    {
        var sb = new StringBuilder();
        var guards = new List<string>();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            if (field.IsRequired && IsStringType(field.Type))
            {
                guards.Add($"        if (string.IsNullOrWhiteSpace(entity.{field.Name}))");
                guards.Add($"            throw new ArgumentException(\"{field.Name} is required\");");
            }

            if (field.MinLength.HasValue && IsStringType(field.Type))
            {
                guards.Add($"        if (entity.{field.Name}.Length < {field.MinLength.Value})");
                guards.Add($"            throw new ArgumentException(\"{field.Name} must be at least {field.MinLength.Value} characters\");");
            }

            if (field.MaxLength.HasValue && IsStringType(field.Type))
            {
                guards.Add($"        if (entity.{field.Name}.Length > {field.MaxLength.Value})");
                guards.Add($"            throw new ArgumentException(\"{field.Name} must be at most {field.MaxLength.Value} characters\");");
            }

            if (field.MinValue.HasValue && IsNumericType(field.Type))
            {
                var minVal = FormatDecimal(field.MinValue.Value);
                guards.Add($"        if (entity.{field.Name} < {minVal})");
                guards.Add($"            throw new ArgumentException(\"{field.Name} must be >= {minVal}\");");
            }

            if (field.MaxValue.HasValue && IsNumericType(field.Type))
            {
                var maxVal = FormatDecimal(field.MaxValue.Value);
                guards.Add($"        if (entity.{field.Name} > {maxVal})");
                guards.Add($"            throw new ArgumentException(\"{field.Name} must be <= {maxVal}\");");
            }

            if (field.IsRelation && !field.IsPivot && field.DeleteBehavior != "SetNull")
            {
                guards.Add($"        if (entity.{field.Name}Id == Guid.Empty)");
                guards.Add($"            throw new ArgumentException(\"{field.Name} is required\");");
            }
        }

        if (guards.Count == 0)
            return "";

        var allGuards = new List<string>
        {
            "        if (entity == null)",
            "            throw new ArgumentNullException(nameof(entity));",
            ""
        };
        allGuards.AddRange(guards);

        sb.AppendLine($"    public override async Task AddAsync({name} entity, object? file = null)");
        sb.AppendLine("    {");
        foreach (var guard in allGuards)
            sb.AppendLine(guard);
        sb.AppendLine();
        sb.AppendLine("        await base.AddAsync(entity, file);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public override async Task UpdateAsync({name} entity, object? file = null)");
        sb.AppendLine("    {");
        foreach (var guard in allGuards)
            sb.AppendLine(guard);
        sb.AppendLine();
        sb.AppendLine("        await base.UpdateAsync(entity, file);");
        sb.AppendLine("    }");

        return sb.ToString();
    }

    private static bool IsStringType(string type)
        => type.Equals("string", StringComparison.OrdinalIgnoreCase);

    private static bool IsNumericType(string type)
        => type.Equals("int", StringComparison.OrdinalIgnoreCase)
            || type.Equals("decimal", StringComparison.OrdinalIgnoreCase)
            || type.Equals("double", StringComparison.OrdinalIgnoreCase)
            || type.Equals("float", StringComparison.OrdinalIgnoreCase)
            || type.Equals("long", StringComparison.OrdinalIgnoreCase);

    private static string FormatDecimal(decimal value)
        => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
