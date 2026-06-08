using UserApp.Infrastructure.Services.CodeGeneration.Shared;

namespace UserApp.Infrastructure.Services.CodeGeneration;

public class MigrationRunner
{
    private readonly CommandRunner _commands;
    private readonly PathProvider _paths;

    public MigrationRunner(CommandRunner commands, PathProvider paths)
    {
        _commands = commands;
        _paths = paths;
    }

    public void AddMigration(string name)
    {
        var migrationName = $"{name}_Auto";
        _commands.Run("dotnet", $"ef migrations add {migrationName} --project {_paths.InfrastructureProject} --startup-project {_paths.WebProject}");
    }

    public void UpdateDatabase()
    {
        _commands.Run("dotnet", $"ef database update --project {_paths.InfrastructureProject} --startup-project {_paths.WebProject}");
    }
}
