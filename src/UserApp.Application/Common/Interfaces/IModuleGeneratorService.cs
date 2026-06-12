using UserApp.Application.Common.DTOs;

namespace UserApp.Application.Common.Interfaces;

public interface IModuleGeneratorService
{
    Task GenerateModuleAsync(
        string moduleName,
        string? systemCode,
        List<ModuleFieldDto> fields,
        bool runMigration = false,
        bool hasImage = false,
        bool runDbUpdate = false
    );
}