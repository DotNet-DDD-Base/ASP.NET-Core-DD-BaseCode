using System.Collections.Generic;
using System.IO;
using System.Text;
using UserApp.Application.Common.DTOs;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class DomainGenerator
{
    private readonly PathProvider _paths;
    private readonly FileManager _files;
    private readonly TemplateEngine _templates;

    public DomainGenerator(PathProvider paths, FileManager files, TemplateEngine templates)
    {
        _paths = paths;
        _files = files;
        _templates = templates;
    }

    public void Generate(string name, List<ModuleFieldDto> fields)
    {
        var domainFolder = Path.Combine(_paths.SrcRoot, "UserApp.Domain", $"{name}s");
        _files.EnsureDirectory(domainFolder);

        var entityContent = _templates.RenderFile(
            new[] { "Domain", "Templates", "Entity.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Properties"] = GenerateProperties(fields)
            });

        _files.WriteFile(Path.Combine(domainFolder, $"{name}.cs"), entityContent);

        var repositoryContent = _templates.RenderFile(
            new[] { "Domain", "Templates", "Repository.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name
            });

        _files.WriteFile(Path.Combine(domainFolder, $"I{name}Repository.cs"), repositoryContent);
    }

    private static string GenerateProperties(List<ModuleFieldDto> fields)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", System.StringComparison.OrdinalIgnoreCase))
                continue;

            var nullable = field.IsNullable && field.Type != "string" ? "?" : string.Empty;
            var property = field.Type == "string"
                ? $"    public string {field.Name} {{ get; set; }} = string.Empty;"
                : $"    public {field.Type}{nullable} {field.Name} {{ get; set; }}";

            sb.AppendLine(property);
        }

        return sb.ToString();
    }
}
