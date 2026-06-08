using System;
using System.IO;
using System.Linq;

namespace UserApp.Infrastructure.Services.CodeGeneration.Shared;

public class PathProvider
{
    public string SolutionRoot { get; }
    public string SrcRoot { get; }
    public string InfrastructureProject => Path.Combine(SolutionRoot, "src", "UserApp.Infrastructure", "UserApp.Infrastructure.csproj");
    public string WebProject => Path.Combine(SolutionRoot, "src", "UserApp.Web", "UserApp.Web.csproj");
    public string CodeGenerationRoot => Path.Combine(SrcRoot, "UserApp.Infrastructure", "Services", "CodeGeneration");
    public string TemplateRoot => CodeGenerationRoot;

    public PathProvider()
    {
        SolutionRoot = LocateSolutionRoot();
        SrcRoot = Path.Combine(SolutionRoot, "src");
    }

    public string TemplatePath(params string[] segments)
    {
        return Path.Combine(new[] { TemplateRoot }.Concat(segments).ToArray());
    }

    private static string LocateSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(dir))
        {
            if (File.Exists(Path.Combine(dir, "UserApp.sln")))
                return dir;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new InvalidOperationException("Solution root not found.");
    }
}
