using System.Collections.Generic;
using System.IO;

namespace UserApp.Infrastructure.Services.CodeGeneration.Shared;

public class TemplateEngine
{
    private readonly PathProvider _paths;

    public TemplateEngine(PathProvider paths)
    {
        _paths = paths;
    }

    public string RenderFile(string[] templatePathSegments, Dictionary<string, string> replacements)
    {
        var templatePath = _paths.TemplatePath(templatePathSegments);
        var template = File.ReadAllText(templatePath);
        return Render(template, replacements);
    }

    public string Render(string template, Dictionary<string, string> replacements)
    {
        foreach (var replacement in replacements)
        {
            template = template.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }

        return template;
    }
}
