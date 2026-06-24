using System.Text.Encodings.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UserApp.Application.AuditLogs.Interfaces;
using UserApp.Web.ViewModels;
using UserApp.Web.ViewModels.AuditLogs;

namespace UserApp.Web.Controllers;

public class AuditLogController : Controller
{
    private readonly IAuditLogService _service;
    private readonly IMapper _mapper;

    public AuditLogController(IAuditLogService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1, int size = 10)
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
            SearchTerm = search ?? string.Empty
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> SearchData(string? search, int page = 1, int size = 10)
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
            SearchTerm = search ?? string.Empty
        };

        return Json(new
        {
            tableHtml = await RenderPartialViewToString("_AuditLogTable", vm),
            paginationHtml = await RenderPartialViewToString("_AuditLogPagination", vm)
        });
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

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<AuditLogViewModel>(entity);
        return View(vm);
    }
}
