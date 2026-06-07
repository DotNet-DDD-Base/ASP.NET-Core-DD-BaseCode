using UserApp.Application.Common.Interfaces;
using UserApp.Application.Common.DTOs;
using System.Text;

namespace UserApp.Infrastructure.Services;

public class ModuleGeneratorService : IModuleGeneratorService
{
    private readonly string _solutionRoot;
    private readonly string _srcPath;

    public ModuleGeneratorService()
    {
        _solutionRoot = GetSolutionRoot();
        _srcPath = Path.Combine(_solutionRoot, "src");
    }

    // ========================= ENTRY =========================
    public Task GenerateModuleAsync(string moduleName, List<ModuleFieldDto> fields)
    {
        var name = Capitalize(moduleName);

        GenerateDomain(name, fields);
        GenerateDomainRepository(name); // ✅ FIX: ADD THIS
        GenerateApplication(name);
        GenerateInfrastructure(name);
        GenerateWeb(name);

        UpdateMappingProfile(name);
        UpdateDbContext(name);
        UpdateProgramCs(name);

        return Task.CompletedTask;
    }

    // ========================= SOLUTION ROOT =========================
    private string GetSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "UserApp.sln")))
                return dir;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new Exception("Solution root not found");
    }

    // ========================= DOMAIN ENTITY =========================
    private void GenerateDomain(string name, List<ModuleFieldDto> fields)
    {
        var path = Path.Combine(_srcPath, $"UserApp.Domain/{name}s");
        Directory.CreateDirectory(path);

        var props = new StringBuilder();

        foreach (var f in fields)
        {
            // ❌ NEVER generate Id (already in Entity<Guid>)
            if (f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                continue;

            var nullable = f.IsNullable && f.Type != "string" ? "?" : "";

            if (f.Type == "string")
            {
                props.AppendLine($@"
    public string {f.Name} {{ get; set; }} = string.Empty;
");
            }
            else
            {
                props.AppendLine($@"
    public {f.Type}{nullable} {f.Name} {{ get; set; }}
");
            }
        }

        var entity = $@"
using UserApp.Domain.Common;

namespace UserApp.Domain.{name}s;

public class {name} : Entity<Guid>
{{
{props}
}}
";

        File.WriteAllText(Path.Combine(path, $"{name}.cs"), entity);
    }

    // ========================= DOMAIN REPOSITORY (IMPORTANT FIX) =========================
    private void GenerateDomainRepository(string name)
    {
        var path = Path.Combine(_srcPath, $"UserApp.Domain/{name}s");
        Directory.CreateDirectory(path);

        var file = Path.Combine(path, $"I{name}Repository.cs");

        var content = $@"
using UserApp.Domain.Common;

namespace UserApp.Domain.{name}s;

public interface I{name}Repository : IBaseRepository<{name}>
{{
}}
";

        File.WriteAllText(file, content);
    }

    // ========================= APPLICATION =========================
    private void GenerateApplication(string name)
    {
        var path = Path.Combine(_srcPath, $"UserApp.Application/{name}s");
        var interfaces = Path.Combine(path, "Interfaces");

        Directory.CreateDirectory(path);
        Directory.CreateDirectory(interfaces);

        File.WriteAllText(Path.Combine(path, $"{name}Service.cs"),
        $@"
using UserApp.Domain.{name}s;
using UserApp.Application.Common;
using UserApp.Application.{name}s.Interfaces;

namespace UserApp.Application.{name}s;

public class {name}Service : BaseService<{name}>, I{name}Service
{{
    public {name}Service(I{name}Repository repo) : base(repo)
    {{
    }}
}}");

        File.WriteAllText(Path.Combine(interfaces, $"I{name}Service.cs"),
        $@"
using UserApp.Application.Common;
using UserApp.Domain.{name}s;

namespace UserApp.Application.{name}s.Interfaces;

public interface I{name}Service : IBaseService<{name}>
{{
}}");
    }

    // ========================= INFRASTRUCTURE =========================
    private void GenerateInfrastructure(string name)
    {
        var path = Path.Combine(_srcPath, "UserApp.Infrastructure/Persistence/Repositories");
        Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, $"{name}Repository.cs"),
        $@"
using UserApp.Domain.{name}s;
using UserApp.Infrastructure.Persistence;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class {name}Repository : BaseRepository<{name}>, I{name}Repository
{{
    public {name}Repository(AppDbContext db) : base(db)
    {{
    }}
}}");
    }

    // ========================= WEB =========================
    private void GenerateWeb(string name)
    {
        var mvc = Path.Combine(_srcPath, "UserApp.Web/Controllers");
        var api = Path.Combine(_srcPath, "UserApp.Web/Controllers/Api");
        var vm = Path.Combine(_srcPath, $"UserApp.Web/ViewModels/{name}s");

        Directory.CreateDirectory(mvc);
        Directory.CreateDirectory(api);
        Directory.CreateDirectory(vm);

        File.WriteAllText(Path.Combine(mvc, $"{name}Controller.cs"),
        $@"
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.{name}s.Interfaces;
using UserApp.Domain.{name}s;
using UserApp.Web.ViewModels.{name}s;

namespace UserApp.Web.Controllers;

public class {name}Controller : BaseController<{name}, {name}ViewModel>
{{
    public {name}Controller(I{name}Service service, IMapper mapper)
        : base(service, mapper)
    {{
    }}
}}");

        File.WriteAllText(Path.Combine(api, $"{name}ApiController.cs"),
        $@"
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.{name}s.Interfaces;
using UserApp.Domain.{name}s;
using UserApp.Web.ViewModels.{name}s;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route(""api/[controller]"")]
[Authorize]
public class {name}ApiController : BaseApiController<{name}, {name}ViewModel>
{{
    public {name}ApiController(I{name}Service service, IMapper mapper)
        : base(service, mapper)
    {{
    }}
}}");

        File.WriteAllText(Path.Combine(vm, $"{name}ViewModel.cs"),
        $@"
namespace UserApp.Web.ViewModels.{name}s;

public class {name}ViewModel
{{
    public Guid Id {{ get; set; }}
    public string Name {{ get; set; }} = string.Empty;
}}");
    }

    // ========================= DB CONTEXT =========================
    private void UpdateDbContext(string name)
    {
        var file = Path.Combine(_srcPath, "UserApp.Infrastructure/Persistence/AppDbContext.cs");

        EnsureUsing(file, $"using UserApp.Domain.{name}s;");

        var inject = $@"
public DbSet<{name}> {name}s => Set<{name}>();
";

        CodeInjector.InjectBetween(file,
            "// <AUTO-DBSETS-START>",
            "// <AUTO-DBSETS-END>",
            inject);
    }

    // ========================= PROGRAM CS =========================
    private void UpdateProgramCs(string name)
    {
        var file = Path.Combine(_srcPath, "UserApp.Web/Program.cs");

        EnsureUsing(file, $"using UserApp.Domain.{name}s;");
        EnsureUsing(file, $"using UserApp.Application.{name}s;");
        EnsureUsing(file, $"using UserApp.Application.{name}s.Interfaces;");
        EnsureUsing(file, $"using UserApp.Infrastructure.Persistence.Repositories;");

        CodeInjector.InjectBetween(file,
            "// <AUTO-REPOSITORIES-START>",
            "// <AUTO-REPOSITORIES-END>",
            $@"builder.Services.AddScoped<I{name}Repository, {name}Repository>();");

        CodeInjector.InjectBetween(file,
            "// <AUTO-SERVICES-START>",
            "// <AUTO-SERVICES-END>",
            $@"builder.Services.AddScoped<I{name}Service, {name}Service>();");
    }

    // ========================= MAPPING =========================
    private void UpdateMappingProfile(string name)
    {
        var file = Path.Combine(_srcPath, "UserApp.Web/Mapping/MappingProfile.cs");

        EnsureUsing(file, $"using UserApp.Domain.{name}s;");
        EnsureUsing(file, $"using UserApp.Web.ViewModels.{name}s;");

        CodeInjector.InjectBetween(file,
            "// <AUTO-MAPPINGS-START>",
            "// <AUTO-MAPPINGS-END>",
            $@"
CreateMap<{name}, {name}ViewModel>();
CreateMap<{name}ViewModel, {name}>();
");
    }

    // ========================= HELPER =========================
    private void EnsureUsing(string filePath, string usingLine)
    {
        var lines = File.ReadAllLines(filePath).ToList();

        if (lines.Any(x => x.Trim() == usingLine))
            return;

        var lastUsing = lines.FindLastIndex(x => x.Trim().StartsWith("using"));

        if (lastUsing == -1)
            throw new Exception("No using found");

        lines.Insert(lastUsing + 1, usingLine);
        File.WriteAllLines(filePath, lines);
    }

    private string Capitalize(string input)
        => char.ToUpper(input[0]) + input.Substring(1);
}