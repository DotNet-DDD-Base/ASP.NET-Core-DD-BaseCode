using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Domain.Common;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public abstract class BaseController<TEntity, TViewModel> : Controller
    where TEntity : class
    where TViewModel : class
{
    private readonly IBaseRepository<TEntity> _repo;
    private readonly IMapper _mapper;

    protected BaseController(IBaseRepository<TEntity> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    // -------------------------
    // INDEX (PAGINATED LIST)
    // -------------------------
    public async Task<IActionResult> Index(int page = 1, int size = 10)
    {
        var data = await _repo.ListAsync((page - 1) * size, size);
        var totalCount = await _repo.CountAsync();

        var items = _mapper.Map<List<TViewModel>>(data);

        var vm = new ListViewModel<TViewModel>
        {
            Page = page,
            PageSize = size,
            TotalCount = totalCount,
            Items = items
        };

        return View("Index", vm);
    }

    // -------------------------
    // DETAILS
    // -------------------------
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        return View("Details", vm);
    }

    // -------------------------
    // CREATE
    // -------------------------
    public IActionResult Create()
        => View("Create");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("Create", vm);

        var entity = _mapper.Map<TEntity>(vm);
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // -------------------------
    // EDIT
    // -------------------------
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        return View("Edit", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("Edit", vm);

        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _mapper.Map(vm, entity);
        _repo.Update(entity);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // -------------------------
    // DELETE
    // -------------------------
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = _mapper.Map<TViewModel>(entity);
        return View("Delete", vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _repo.Remove(entity);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}