using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UserApp.Application.Common.Interfaces;

namespace UserApp.Web.Common;

public class PermissionFilter : IAsyncActionFilter
{
    private readonly IPermissionChecker _permissionService;

    public PermissionFilter(IPermissionChecker permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // =====================================================
        // 1. SKIP API ROUTES (authenticated via JWT)
        // =====================================================
        if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            await next();
            return;
        }

        // =====================================================
        // 2. SKIP IF [AllowAnonymous]
        // =====================================================
        var endpoint = context.ActionDescriptor.EndpointMetadata;

        if (endpoint.Any(x => x is AllowAnonymousAttribute))
        {
            await next();
            return;
        }

        // =====================================================
        // 2. AUTH CHECK
        // =====================================================
        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userId = Guid.Parse(userIdClaim);

        // =====================================================
        // 3. BUILD PERMISSION FROM ROUTE
        // =====================================================
        var controller = context.ActionDescriptor.RouteValues["controller"];
        var action = context.ActionDescriptor.RouteValues["action"];

        if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
        {
            await next();
            return;
        }

        var permission = $"{controller}.{action}";

        // =====================================================
        // 4. CHECK DB PERMISSION
        // =====================================================
        var hasPermission = await _permissionService.HasPermissionAsync(
            userId,
            permission
        );

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }

        // =====================================================
        // 5. ALLOW REQUEST
        // =====================================================
        await next();
    }
}