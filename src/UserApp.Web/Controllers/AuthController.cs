using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Users.DTOs;
using UserApp.Application.Users.Interfaces;
using UserApp.Web.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UserApp.Domain.Roles;
using UserApp.Domain.Common;

namespace UserApp.Web.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IBaseRepository<UserRole> _userRoleBaseRepository;
    private readonly IBaseRepository<Role> _roleBaseRepository;

    public AuthController(
        IAuthService authService,
        IBaseRepository<UserRole> userRoleBaseRepository,
        IBaseRepository<Role> roleBaseRepository)
    {
        _authService = authService;
        _userRoleBaseRepository = userRoleBaseRepository;
        _roleBaseRepository = roleBaseRepository;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        // 1. Authenticate via application layer
        var authResult = await _authService.LoginAsync(new LoginDto(vm.Email, vm.Password));

        if (authResult == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        // 2. Fetch the full domain entity to access the Guid Id
        var userEntity = await _authService.GetUserByEmailAsync(authResult.Email);
        if (userEntity == null)
        {
            ModelState.AddModelError("", "User profile synchronization failed.");
            return View(vm);
        }

        // 3. Build basic identity claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userEntity.Id.ToString()),
            new Claim(ClaimTypes.Name, authResult.FullName),
            new Claim(ClaimTypes.Email, authResult.Email)
        };

        // 4. Use generic base repository to get user roles securely
        var allUserRoles = await _userRoleBaseRepository.ListAsync(0, 1000);
        var assignedRoleIds = allUserRoles
            .Where(ur => ur.UserId == userEntity.Id)
            .Select(ur => ur.RoleId)
            .ToList();

        // 5. Fetch Role names based on the assigned IDs
        foreach (var roleId in assignedRoleIds)
        {
            var role = await _roleBaseRepository.GetByIdAsync(roleId);
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // 6. Issue the browser cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Users");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _authService.RegisterAsync(new RegisterDto(vm.Email, vm.FullName, vm.Password));

            // 🔥 FIX: Explicitly target the Web Controller's Login view
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Denied()
    {
        return View();
    }
}