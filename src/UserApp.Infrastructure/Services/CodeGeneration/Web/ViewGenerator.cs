using System.Text;
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

    public void GenerateViews(string name, List<ModuleFieldDto> fields, bool hasImage)
    {
        var viewFolder = Path.Combine(_paths.SrcRoot, "UserApp.Web", "Views", name);
        _files.EnsureDirectory(viewFolder);

        var indexContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Index.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Columns"] = BuildTableColumns(fields, hasImage),
                ["Rows"] = BuildTableRows(fields, hasImage)
            });

        var createContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Create.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["Inputs"] = BuildFormInputs(fields, hasImage)
            });

        var editContent = _templates.RenderFile(
            new[] { "Web", "Templates", "Edit.tpl" },
            new Dictionary<string, string>
            {
                ["Name"] = name,
                ["CurrentImages"] = BuildCurrentImages(hasImage),
                ["Inputs"] = BuildFormInputs(fields, hasImage)
            });

        _files.WriteFile(Path.Combine(viewFolder, "Index.cshtml"), indexContent);
        _files.WriteFile(Path.Combine(viewFolder, "Create.cshtml"), createContent);
        _files.WriteFile(Path.Combine(viewFolder, "Edit.cshtml"), editContent);
    }

    private static string BuildTableColumns(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($"<th>{field.Name}</th>");
        }

        if (hasImage)
            sb.AppendLine("<th>Images</th>");

        return sb.ToString();
    }

    private static string BuildTableRows(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($"<td>@p.{field.Name}</td>");
        }

        if (hasImage)
        {
            sb.AppendLine(@"<td>
    @if (p.ImageUrls.Count > 0)
    {
        <div style=""display:flex; gap:4px;"">
        @foreach (var img in p.ImageUrls.Take(3))
        {
            <img src=""@img"" width=""50"" height=""50"" style=""object-fit:cover;"" />
        }
        </div>
    }
</td>");
        }

        return sb.ToString();
    }

    private static string BuildFormInputs(List<ModuleFieldDto> fields, bool hasImage)
    {
        var sb = new StringBuilder();

        foreach (var field in fields)
        {
            if (field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            sb.AppendLine($@"
<div class=""form-group"">
    <label>{field.Name}</label>
    <input asp-for=""{field.Name}"" class=""form-control"" />
    <span asp-validation-for=""{field.Name}"" class=""text-danger""></span>
</div>");
        }

        // ✅ IMAGE BLOCK: real file upload for generated forms
        if (hasImage)
        {
            sb.AppendLine(@"
<div class=""form-group border p-2 mt-3"">
    <label>Images</label>
    <input type=""file"" name=""files"" multiple class=""form-control"" />
    <small class=""text-muted"">Choose one or more image files to upload for this item.</small>
</div>");
        }

        return sb.ToString();
    }

    internal void GenerateViews(string name, List<ModuleFieldDto> fields, object hasImage)
    {
        throw new NotImplementedException();
    }

    private static string BuildCurrentImages(bool hasImage)
    {
        if (!hasImage) return string.Empty;

        return @"
@if (Model.ImageUrls.Count > 0)
{
    <div class=""form-group border p-2 mt-3"">
        <label>Current Images</label>
        <div style=""display:flex; gap:8px; flex-wrap:wrap; margin-top:8px;"">
        @foreach (var img in Model.ImageUrls)
        {
            <div>
                <img src=""@img"" width=""100"" height=""100"" style=""object-fit:cover; border:1px solid #ddd; border-radius:4px;"" />
            </div>
        }
        </div>
    </div>
}
";
    }

    private string BuildProperty(ModuleFieldDto field)
    {
        var sb = new StringBuilder();

        if (field.IsRequired)
            sb.AppendLine("[Required(ErrorMessage = \"" + field.Name + " is required\")]");

        if (field.MinLength.HasValue || field.MaxLength.HasValue)
        {
            sb.AppendLine(
                $"[StringLength({field.MaxLength ?? 500}," +
                $" MinimumLength = {field.MinLength ?? 0}," +
                $" ErrorMessage = \"{field.Name} length must be between {field.MinLength ?? 0} and {field.MaxLength ?? 500}\")]");
        }

        if (field.Type == "decimal" ||
            field.Type == "double" ||
            field.Type == "int")
        {
            sb.AppendLine(
                $"[Range({field.MinValue ?? 0}," +
                $"{field.MaxValue ?? 999999999}," +
                $"ErrorMessage = \"{field.Name} must be between {field.MinValue ?? 0} and {field.MaxValue ?? 999999999}\")]");
        }

        sb.AppendLine(
            $"public {field.Type}{(field.IsNullable ? "?" : "")} {field.Name} {{ get; set; }}");

        return sb.ToString();
    }
}
