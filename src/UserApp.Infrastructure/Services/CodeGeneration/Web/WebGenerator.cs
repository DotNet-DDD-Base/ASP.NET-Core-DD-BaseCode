using System.Collections.Generic;
using System.IO;
using System.Text;
using UserApp.Application.Common.DTOs;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class WebGenerator
{
    private readonly PathProvider _paths;
    private readonly FileManager _files;
    private readonly TemplateEngine _templates;
    private readonly ViewGenerator _views;


    public WebGenerator(PathProvider paths, FileManager files, TemplateEngine templates)
    {
        _paths = paths;
        _files = files;
        _templates = templates;
        _views = new ViewGenerator(_paths, _files, _templates);
    }

    public void Generate(string name, List<ModuleFieldDto> fields, bool hasImage)
    {
        var controllersFolder = Path.Combine(_paths.SrcRoot, "UserApp.Web", "Controllers");
        var apiControllersFolder = Path.Combine(controllersFolder, "Api");
        var viewModelsFolder = Path.Combine(_paths.SrcRoot, "UserApp.Web", "ViewModels", $"{name}s");

        _files.EnsureDirectory(controllersFolder);
        _files.EnsureDirectory(apiControllersFolder);
        _files.EnsureDirectory(viewModelsFolder);

        var controllerContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Controller.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name
            });

        var apiControllerContent = _templates.RenderFile(
            new[] { "Web", "Templates", "ApiController.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name
            });

        var viewModelContent = _templates.RenderFile(
            new[] { "Web", "Templates", "ViewModel.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Properties"] = BuildViewModelProperties(fields, hasImage)
            });

        _files.WriteFile(Path.Combine(controllersFolder, $"{name}Controller.cs"), controllerContent);
        _files.WriteFile(Path.Combine(apiControllersFolder, $"{name}ApiController.cs"), apiControllerContent);
        _files.WriteFile(Path.Combine(viewModelsFolder, $"{name}ViewModel.cs"), viewModelContent);

        _views.GenerateViews(name, fields, hasImage);
    }

    private static string BuildViewModelProperties(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            var type = field.Type;
            var name = field.Name;

            if (type == "string")
            {
                sb.AppendLine($"    public string {name} {{ get; set; }} = string.Empty;");
            }
            else
            {
                var nullable = field.IsNullable ? "?" : string.Empty;
                sb.AppendLine($"    public {type}{nullable} {name} {{ get; set; }}");
            }
        }

        // ✅ ADD IMAGE FIELD (from Media table, not DB column)
        if (hasImage)
        {
            sb.AppendLine();
            sb.AppendLine("    public string? ImageUrl { get; set; }");
            sb.AppendLine("    public Guid? MediaId { get; set; }");
        }

        return sb.ToString();
    }
}
