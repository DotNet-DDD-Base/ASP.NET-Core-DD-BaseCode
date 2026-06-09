using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Common;
using UserApp.Application.Common.Interfaces;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public abstract class BaseController<TEntity, TViewModel> : Controller
    where TEntity : class
    where TViewModel : class
{
    protected readonly IBaseService<TEntity> _service;
    protected readonly IMapper _mapper;
    protected readonly IMediaService? _mediaService;


    protected BaseController(
        IBaseService<TEntity> service,
        IMapper mapper,
        IMediaService? mediaService = null)
    {
        _service = service;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    // -------------------------
    // INDEX
    // -------------------------
    public async Task<IActionResult> Index(int page = 1, int size = 10)
    {
        var data = await _service.ListAsync((page - 1) * size, size);
        var totalCount = await _service.CountAsync();

        var items = _mapper.Map<List<TViewModel>>(data);

        if (_mediaService != null)
        {
            foreach (var item in items)
            {
                var idProp = item.GetType().GetProperty("Id");
                var imgProp = item.GetType().GetProperty("ImageUrl");

                if (idProp == null || imgProp == null) continue;

                var id = (Guid)idProp.GetValue(item)!;

                var url = await _mediaService.GetLatestUrlAsync(
                    typeof(TEntity).Name,
                    id);

                imgProp.SetValue(item, url);
            }
        }

        return View("Index", new ListViewModel<TViewModel>
        {
            Page = page,
            PageSize = size,
            TotalCount = totalCount,
            Items = items
        });
    }
    // -------------------------
    // DETAILS
    // -------------------------
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        return View("Details", vm);
    }

    // -------------------------
    // CREATE
    public IActionResult Create()
        => View("Create");    // -------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TViewModel vm, IFormFile? file)
    {
        if (!ModelState.IsValid)
            return View("Create", vm);

        var entity = _mapper.Map<TEntity>(vm);

        await _service.AddAsync(entity, file);

        return RedirectToAction(nameof(Index));
    }

    // -------------------------
    // EDIT
    // -------------------------
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);

        // 🔥 LOAD IMAGE FROM MEDIA TABLE
        if (_mediaService != null)
        {
            var url = await _mediaService.GetLatestUrlAsync(
                typeof(TEntity).Name,
                id);

            var prop = vm.GetType().GetProperty("ImageUrl");
            if (prop != null)
            {
                prop.SetValue(vm, url);
            }
        }

        return View("Edit", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TViewModel vm, IFormFile? file)
    {
        if (!ModelState.IsValid)
            return View("Edit", vm);

        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _mapper.Map(vm, entity);

        await _service.UpdateAsync(entity, file);

        return RedirectToAction(nameof(Index));
    }

    // -------------------------
    // DELETE
    // -------------------------
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        return View("Delete", vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        await _service.RemoveAsync(entity);

        return RedirectToAction(nameof(Index));
    }
}