using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserApp.Application.Common;
using UserApp.Application.Common.DTOs;
using UserApp.Application.Common.Interfaces;
using UserApp.Infrastructure.Persistence;
using UserApp.Application.SidebarGroups.Interfaces;
using UserApp.Web.ViewModels.ModuleGenerator;

namespace UserApp.Web.Controllers.Api;

[ApiController]
[Route("api/module-generator")]
[AllowAnonymous]
public class ModuleGeneratorApiController : ControllerBase
{
    private readonly IModuleGeneratorService _service;
    private readonly AppDbContext _db;
    private readonly ISidebarGroupService _sidebarGroupService;

    public ModuleGeneratorApiController(
        IModuleGeneratorService service,
        AppDbContext db,
        ISidebarGroupService sidebarGroupService)
    {
        _service = service;
        _db = db;
        _sidebarGroupService = sidebarGroupService;
    }

    [HttpGet("tables")]
    public ActionResult<ApiResponse<List<string>>> GetTables()
    {
        var tables = _db.Model.GetEntityTypes()
            .Select(e => e.ClrType.Name)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        return Ok(ApiResponse<List<string>>.Ok(tables, "Tables retrieved successfully"));
    }

    [HttpGet("tables/{tableName}/fields")]
    public ActionResult<ApiResponse<List<string>>> GetTableFields(string tableName)
    {
        var domainAssembly = System.Reflection.Assembly.Load("UserApp.Domain");
        var entityType = domainAssembly.GetTypes()
            .FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)
                && t is { IsClass: true, IsAbstract: false }
                && t.Namespace != null
                && t.Namespace.StartsWith("UserApp.Domain"));

        if (entityType == null)
            return NotFound(ApiResponse<List<string>>.Fail($"Table '{tableName}' not found"));

        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "CreatedAt", "UpdatedAt", "DeletedAt", "IsDeleted"
        };

        var fields = entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => !excluded.Contains(p.Name)
                && (p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                && !p.PropertyType.IsGenericType)
            .Select(p => p.Name)
            .OrderBy(n => n)
            .ToList();

        return Ok(ApiResponse<List<string>>.Ok(fields, "Fields retrieved successfully"));
    }

    [HttpGet("tables/{tableName}/data")]
    public async Task<ActionResult<ApiResponse<List<Dictionary<string, object>>>>> GetTableData(string tableName, int count = 5)
    {
        var domainAssembly = System.Reflection.Assembly.Load("UserApp.Domain");
        var entityType = domainAssembly.GetTypes()
            .FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)
                && t is { IsClass: true, IsAbstract: false }
                && t.Namespace != null
                && t.Namespace.StartsWith("UserApp.Domain"));

        if (entityType == null)
            return NotFound(ApiResponse<List<Dictionary<string, object>>>.Fail($"Table '{tableName}' not found"));

        try
        {
            var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!
                .MakeGenericMethod(entityType);
            var dbSet = setMethod.Invoke(_db, null);

            var toList = typeof(Enumerable).GetMethod("ToList")!
                .MakeGenericMethod(entityType);
            var all = (IEnumerable<object>)toList.Invoke(null, [dbSet])!;

            var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id", "CreatedAt", "UpdatedAt", "DeletedAt", "IsDeleted"
            };

            var results = all
                .Take(count)
                .Select(e =>
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (excluded.Contains(prop.Name)) continue;
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string)) continue;
                        if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)) continue;

                        var val = prop.GetValue(e);
                        dict[prop.Name] = val ?? "";
                    }
                    return dict;
                })
                .ToList();

            return Ok(ApiResponse<List<Dictionary<string, object>>>.Ok(results, "Data retrieved successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<List<Dictionary<string, object>>>.Fail($"Could not load data: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Generate(
        [FromBody] GenerateModuleViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                ApiResponse<object>.Fail("Invalid request"));

        var moduleName = vm.ModuleName?.Trim();

        if (string.IsNullOrWhiteSpace(moduleName))
            return BadRequest(
                ApiResponse<object>.Fail("Module name is required"));

        var fields = vm.Fields
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new ModuleFieldDto
            {
                Name = x.Name,
                Type = x.Type,
                Length = x.Length,
                IsRequired = x.IsRequired,
                IsNullable = x.IsNullable,
                MinLength = x.MinLength,
                MaxLength = x.MaxLength,
                MinValue = x.MinValue,
                MaxValue = x.MaxValue,
                EnumValues = x.EnumValues,
                UseCommonTable = x.UseCommonTable,
                EnumRenderAsCheckbox = x.EnumRenderAsCheckbox,
                IsRelation = x.IsRelation,
                RelatedEntityName = x.RelatedEntityName,
                IsPivot = x.IsPivot,
                DeleteBehavior = x.DeleteBehavior,
                DisplayField = x.DisplayField
            })
            .ToList();

        Guid? sidebarGroupId = null;
        if (vm.EnableSidebar)
        {
            var groups = await _sidebarGroupService.GetAllOrderedAsync();
            sidebarGroupId = groups.FirstOrDefault()?.Id;
        }

        await _service.GenerateModuleAsync(
            moduleName,
            fields,
            vm.RunMigration,
            vm.HasImage,
            vm.RunDbUpdate,
            sidebarGroupId);

        return Ok(
            ApiResponse<object>.Ok(
                null,
                $"{moduleName} module generated successfully"));
    }
}
