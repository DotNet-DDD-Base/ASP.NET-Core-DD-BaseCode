using System.Collections.Generic;
using System.IO;
using UserApp.Application.Common.DTOs;
using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class ViewGenerator
{
    private readonly PathProvider _paths;
    private readonly FileManager _files;
    private readonly TemplateEngine _templates;

    public ViewGenerator(PathProvider paths, FileManager files, TemplateEngine templates)
    {
        _paths = paths;
        _files = files;
        _templates = templates;
    }

    public void GenerateViews(string name, List<ModuleFieldDto> fields)
    {
        var viewFolder = Path.Combine(_paths.SrcRoot, "UserApp.Web", "Views", name);
        _files.EnsureDirectory(viewFolder);

        var indexContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Index.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Columns"] = BuildTableColumns(fields),
                ["Rows"] = BuildTableRows(fields)
            });

        var createContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Create.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Inputs"] = BuildFormInputs(fields)
            });

        var editContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Edit.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Inputs"] = BuildFormInputs(fields)
            });

        _files.WriteFile(Path.Combine(viewFolder, "Index.cshtml"), indexContent);
        _files.WriteFile(Path.Combine(viewFolder, "Create.cshtml"), createContent);
        _files.WriteFile(Path.Combine(viewFolder, "Edit.cshtml"), editContent);
    }

    private static string BuildTableColumns(List<ModuleFieldDto> fields)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", System.StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($"                    <th>{field.Name}</th>");
        }

        return sb.ToString();
    }

    private static string BuildTableRows(List<ModuleFieldDto> fields)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", System.StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($"                    <td>@p.{field.Name}</td>");
        }

        return sb.ToString();
    }

    private static string BuildFormInputs(List<ModuleFieldDto> fields)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", System.StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($@"
        <div class=""form-group"">
            <label>{field.Name}</label>
            <input asp-for=""{field.Name}"" class=""form-control"" />
        </div>");
        }

        return sb.ToString();
    }
}
