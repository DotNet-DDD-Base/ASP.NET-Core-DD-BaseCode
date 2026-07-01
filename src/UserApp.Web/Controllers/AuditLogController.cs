using System.Reflection;
using System.Text.Encodings.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UserApp.Application.AuditLogs.Interfaces;
using UserApp.Application.Common;
using UserApp.Domain.AuditLogs;
using UserApp.Domain.Common;
using UserApp.Web.ViewModels;
using UserApp.Web.ViewModels.AuditLogs;

namespace UserApp.Web.Controllers;

public class AuditLogController : Controller
{
    private readonly IAuditLogService _service;
    private readonly IAuditLogArchiveRepository _archiveRepo;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public AuditLogController(
        IAuditLogService service,
        IAuditLogArchiveRepository archiveRepo,
        IMapper mapper,
        IServiceProvider serviceProvider)
    {
        _service = service;
        _archiveRepo = archiveRepo;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1, int size = 10,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var todayStart = TimeHelper.Now.Date;
        var from = fromDate?.Date ?? todayStart;
        var to = toDate?.Date.AddDays(1) ?? todayStart.AddDays(1);

        if (from >= todayStart)
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

            var vm = new ListViewModel<AuditLogViewModel>
            {
                Items = _mapper.Map<List<AuditLogViewModel>>(items),
                Page = page,
                PageSize = size,
                TotalCount = total,
                SearchTerm = search ?? string.Empty,
                FromDate = from,
                ToDate = to.AddDays(-1)
            };
            return View(vm);
        }
        else
        {
            List<AuditLogArchive> items;
            int total;

            if (string.IsNullOrWhiteSpace(search))
            {
                items = await _archiveRepo.ListForDateRangeAsync(from, to, (page - 1) * size, size);
                total = await _archiveRepo.CountForDateRangeAsync(from, to);
            }
            else
            {
                items = await _archiveRepo.SearchForDateRangeAsync(search, from, to, (page - 1) * size, size);
                total = await _archiveRepo.CountSearchForDateRangeAsync(search, from, to);
            }

            var vm = new ListViewModel<AuditLogViewModel>
            {
                Items = _mapper.Map<List<AuditLogViewModel>>(items),
                Page = page,
                PageSize = size,
                TotalCount = total,
                SearchTerm = search ?? string.Empty,
                FromDate = from,
                ToDate = to.AddDays(-1)
            };
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchData(string? search, int page = 1, int size = 10,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var todayStart = TimeHelper.Now.Date;
        var from = fromDate?.Date ?? todayStart;
        var to = toDate?.Date.AddDays(1) ?? todayStart.AddDays(1);

        if (from >= todayStart)
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

            var vm = new ListViewModel<AuditLogViewModel>
            {
                Items = _mapper.Map<List<AuditLogViewModel>>(items),
                Page = page,
                PageSize = size,
                TotalCount = total,
                SearchTerm = search ?? string.Empty,
                FromDate = from,
                ToDate = to.AddDays(-1)
            };

            return Json(new
            {
                tableHtml = await RenderPartialViewToString("_AuditLogTable", vm),
                paginationHtml = await RenderPartialViewToString("_AuditLogPagination", vm)
            });
        }
        else
        {
            List<AuditLogArchive> items;
            int total;

            if (string.IsNullOrWhiteSpace(search))
            {
                items = await _archiveRepo.ListForDateRangeAsync(from, to, (page - 1) * size, size);
                total = await _archiveRepo.CountForDateRangeAsync(from, to);
            }
            else
            {
                items = await _archiveRepo.SearchForDateRangeAsync(search, from, to, (page - 1) * size, size);
                total = await _archiveRepo.CountSearchForDateRangeAsync(search, from, to);
            }

            var vm = new ListViewModel<AuditLogViewModel>
            {
                Items = _mapper.Map<List<AuditLogViewModel>>(items),
                Page = page,
                PageSize = size,
                TotalCount = total,
                SearchTerm = search ?? string.Empty,
                FromDate = from,
                ToDate = to.AddDays(-1)
            };

            return Json(new
            {
                tableHtml = await RenderPartialViewToString("_AuditLogTable", vm),
                paginationHtml = await RenderPartialViewToString("_AuditLogPagination", vm)
            });
        }
    }

    public async Task<IActionResult> Details(Guid id, DateTime? fromDate = null)
    {
        var todayStart = TimeHelper.Now.Date;

        if (fromDate.HasValue && fromDate.Value.Date < todayStart)
        {
            var entity = await _archiveRepo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var vm = _mapper.Map<AuditLogViewModel>(entity);
            ViewBag.FromDate = fromDate.Value;
            return View(vm);
        }

        var current = await _service.GetByIdAsync(id);
        if (current == null) return NotFound();

        return View(_mapper.Map<AuditLogViewModel>(current));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, DateTime? fromDate = null)
    {
        var todayStart = TimeHelper.Now.Date;

        Domain.AuditLogs.AuditLog? auditLog = null;
        AuditLogArchive? archiveLog = null;

        if (fromDate.HasValue && fromDate.Value.Date < todayStart)
        {
            archiveLog = await _archiveRepo.GetByIdAsync(id);
        }
        else
        {
            auditLog = await _service.GetByIdAsync(id);
        }

        var log = auditLog as object ?? archiveLog as object;
        if (log == null) return NotFound();

        var entityName = log is Domain.AuditLogs.AuditLog al ? al.EntityName : (log as AuditLogArchive)!.EntityName;
        var entityId = log is Domain.AuditLogs.AuditLog al2 ? al2.EntityId : (log as AuditLogArchive)!.EntityId;

        var service = ResolveEntityService(entityName);
        if (service == null) return NotFound();

        var restoreMethod = typeof(IBaseService<>)
            .MakeGenericType(service.GetType().GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService<>))
                .GetGenericArguments()[0])
            .GetMethod(nameof(IBaseService<object>.RestoreAsync));

        if (restoreMethod == null) return NotFound();

        await (Task)restoreMethod.Invoke(service, [Guid.Parse(entityId)])!;

        TempData["Success"] = $"{entityName} restored successfully.";
        return RedirectToAction(nameof(Index), new { fromDate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revert(Guid id, DateTime? fromDate = null)
    {
        var todayStart = TimeHelper.Now.Date;

        Domain.AuditLogs.AuditLog? auditLog = null;
        AuditLogArchive? archiveLog = null;

        if (fromDate.HasValue && fromDate.Value.Date < todayStart)
        {
            archiveLog = await _archiveRepo.GetByIdAsync(id);
        }
        else
        {
            auditLog = await _service.GetByIdAsync(id);
        }

        var log = auditLog as object ?? archiveLog as object;
        if (log == null) return NotFound();

        var entityName = log is Domain.AuditLogs.AuditLog al ? al.EntityName : (log as AuditLogArchive)!.EntityName;
        var oldValues = log is Domain.AuditLogs.AuditLog al2 ? al2.OldValues : (log as AuditLogArchive)!.OldValues;

        if (string.IsNullOrEmpty(oldValues))
        {
            TempData["Warning"] = "No previous values available to revert for this entry.";
            return RedirectToAction(nameof(Index), new { fromDate });
        }

        var service = ResolveEntityService(entityName);
        if (service == null) return NotFound();

        var revertMethod = typeof(IBaseService<>)
            .MakeGenericType(service.GetType().GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService<>))
                .GetGenericArguments()[0])
            .GetMethod(nameof(IBaseService<object>.RevertFromAuditAsync));

        if (revertMethod == null) return NotFound();

        try
        {
            await (Task)revertMethod.Invoke(service, [id])!;
        }
        catch (Exception)
        {
            TempData["Warning"] = "Could not revert. The record may have been deleted or changed.";
            return RedirectToAction(nameof(Index), new { fromDate });
        }

        TempData["Success"] = $"{entityName} reverted to previous values.";
        return RedirectToAction(nameof(Index), new { fromDate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TestArchive()
    {
        var archiveService = HttpContext.RequestServices.GetRequiredService<IAuditLogArchiveService>();
        await archiveService.ArchiveYesterdayAsync();
        TempData["Success"] = "Archive job completed. Check logs for details.";
        return RedirectToAction(nameof(Index));
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

    private async Task<string> RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;
        using var sw = new StringWriter();
        var engine = HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        var viewResult = engine.FindView(ControllerContext, viewName, isMainPage: false);
        if (viewResult.View == null) return string.Empty;
        var viewContext = new ViewContext(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            sw,
            new HtmlHelperOptions()
        );
        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}
