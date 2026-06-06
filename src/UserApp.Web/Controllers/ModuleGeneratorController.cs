using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Common.Interfaces;
using UserApp.Web.ViewModels.ModuleGenerator;

namespace UserApp.Web.Controllers;

public class ModuleGeneratorController : Controller
{
    private readonly IModuleGeneratorService _service;

    public ModuleGeneratorController(IModuleGeneratorService service)
    {
        _service = service;
    }

    // =========================
    // GET
    // =========================
    [HttpGet]
    public IActionResult Index()
    {
        return View(new GenerateModuleViewModel());
    }

    // =========================
    // POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(GenerateModuleViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var moduleName = vm.ModuleName?.Trim();

        if (string.IsNullOrWhiteSpace(moduleName))
        {
            ModelState.AddModelError(nameof(vm.ModuleName), "Module name is required.");
            return View(vm);
        }

        await _service.GenerateModuleAsync(moduleName);

        TempData["Success"] = $"{moduleName} module generated successfully!";

        return RedirectToAction(nameof(Index));
    }
}