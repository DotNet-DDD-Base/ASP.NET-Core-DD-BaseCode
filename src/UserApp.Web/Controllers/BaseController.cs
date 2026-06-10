using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using UserApp.Application.Common;
using UserApp.Application.Common.Interfaces;
using UserApp.Web.Common;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public abstract class BaseController<TEntity, TViewModel> : Controller
    where TEntity : class
    where TViewModel : class
{
    protected readonly IBaseService<TEntity> _service;
    protected readonly IMapper _mapper;
    protected readonly IMediaService? _mediaService;

    private IMediaService? MediaService =>
        _mediaService ?? HttpContext?.RequestServices.GetService<IMediaService>();

    protected BaseController(
        IBaseService<TEntity> service,
        IMapper mapper,
        IMediaService? mediaService = null)
    {
        _service = service;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    private async Task LoadImageUrls(object vm, Guid? entityId = null)
    {
        var mediaService = MediaService;
        if (mediaService == null) return;

        var idProp = vm.GetType().GetProperty("Id");
        var imgProp = vm.GetType().GetProperty("ImageUrls");
        if (idProp == null || imgProp == null) return;

        var id = entityId ?? (Guid)idProp.GetValue(vm)!;
        var media = await mediaService.GetAsync(typeof(TEntity).Name, id);
        var urls = media.Select(x => x.Url).ToList();
        imgProp.SetValue(vm, urls);
    }

    public async Task<IActionResult> Index(int page = 1, int size = 10)
    {
        var data = await _service.ListAsync((page - 1) * size, size);
        var totalCount = await _service.CountAsync();

        var items = _mapper.Map<List<TViewModel>>(data);

        foreach (var item in items)
        {
            await LoadImageUrls(item);
        }

        return View("Index", new ListViewModel<TViewModel>
        {
            Page = page,
            PageSize = size,
            TotalCount = totalCount,
            Items = items
        });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        await LoadImageUrls(vm, id);

        return View("Details", vm);
    }

    public IActionResult Create()
        => View("Create");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create(TViewModel vm, List<IFormFile>? files = null)
    {
        if (!ValidateModel(vm))
            return View(vm);

        var entity = _mapper.Map<TEntity>(vm);

        await _service.AddAsync(entity, files);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        await LoadImageUrls(vm, id);

        return View("Edit", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TViewModel vm, List<IFormFile>? files = null)
    {
        if (!ModelState.IsValid)
            return View("Edit", vm);

        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _mapper.Map(vm, entity);

        await _service.UpdateAsync(entity, files);

        return RedirectToAction(nameof(Index));
    }

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

    protected bool ValidateModel<T>(T model)
    {
        var results = DynamicValidator.Validate(model);

        foreach (var error in results)
        {
            foreach (var member in error.MemberNames)
            {
                ModelState.AddModelError(member, error.ErrorMessage!);
            }
        }

        return ModelState.IsValid;
    }
}