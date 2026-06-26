using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.AuditLogs.Interfaces;
using UserApp.Application.Common;
using UserApp.Domain.Common;
using UserApp.Web.ViewModels.AuditLogs;

namespace UserApp.Web.Controllers.Api;

[Route("api/audit-log")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AuditLogApiController : ControllerBase
{
    private readonly IAuditLogService _service;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public AuditLogApiController(IAuditLogService service, IMapper mapper, IServiceProvider serviceProvider)
    {
        _service = service;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        List<Domain.AuditLogs.AuditLog> items;
        int total;

        if (string.IsNullOrWhiteSpace(search))
        {
            items = await _service.ListAsync((page - 1) * size, size);
            total = await _service.CountAsync();
        }
        else
        {
            items = await _service.SearchAsync(search, (page - 1) * size, size);
            total = await _service.CountSearchAsync(search);
        }

        var data = new
        {
            items = _mapper.Map<List<AuditLogViewModel>>(items),
            page,
            pageSize = size,
            totalCount = total,
            totalPages = (int)Math.Ceiling(total / (double)size)
        };

        return Ok(ApiResponse<object>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
            return NotFound(ApiResponse<object>.Fail("Audit log not found."));

        return Ok(ApiResponse<object>.Ok(_mapper.Map<AuditLogViewModel>(entity)));
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var auditLog = await _service.GetByIdAsync(id);
        if (auditLog == null)
            return NotFound(ApiResponse<object>.Fail("Audit log not found."));

        var service = ResolveEntityService(auditLog.EntityName);
        if (service == null)
            return BadRequest(ApiResponse<object>.Fail($"No service registered for entity '{auditLog.EntityName}'."));

        var serviceInterface = service.GetType().GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService<>));

        var restoreMethod = typeof(IBaseService<>)
            .MakeGenericType(serviceInterface.GetGenericArguments()[0])
            .GetMethod(nameof(IBaseService<object>.RestoreAsync));

        if (restoreMethod == null)
            return BadRequest(ApiResponse<object>.Fail("Restore method not found."));

        await (Task)restoreMethod.Invoke(service, [Guid.Parse(auditLog.EntityId)])!;

        return Ok(ApiResponse<object>.Ok(null, $"{auditLog.EntityName} restored successfully."));
    }

    [HttpPost("{id:guid}/revert")]
    public async Task<IActionResult> Revert(Guid id)
    {
        var auditLog = await _service.GetByIdAsync(id);
        if (auditLog == null)
            return NotFound(ApiResponse<object>.Fail("Audit log not found."));

        if (string.IsNullOrEmpty(auditLog.OldValues))
            return BadRequest(ApiResponse<object>.Fail("No previous values available to revert for this entry."));

        var service = ResolveEntityService(auditLog.EntityName);
        if (service == null)
            return BadRequest(ApiResponse<object>.Fail($"No service registered for entity '{auditLog.EntityName}'."));

        var serviceInterface = service.GetType().GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService<>));

        var revertMethod = typeof(IBaseService<>)
            .MakeGenericType(serviceInterface.GetGenericArguments()[0])
            .GetMethod(nameof(IBaseService<object>.RevertFromAuditAsync));

        if (revertMethod == null)
            return BadRequest(ApiResponse<object>.Fail("Revert method not found."));

        try
        {
            await (Task)revertMethod.Invoke(service, [id])!;
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse<object>.Fail("Could not revert. The record may have been deleted or changed."));
        }

        return Ok(ApiResponse<object>.Ok(null, $"{auditLog.EntityName} reverted to previous values."));
    }

    private object? ResolveEntityService(string entityName)
    {
        var domainAssembly = typeof(Entity<Guid>).Assembly;
        var entityType = domainAssembly.GetTypes()
            .FirstOrDefault(t => t.Name == entityName && !t.IsAbstract && t.IsSubclassOf(typeof(Entity<Guid>)));

        if (entityType == null) return null;

        var serviceType = typeof(IBaseService<>).MakeGenericType(entityType);
        return _serviceProvider.GetService(serviceType);
    }
}
