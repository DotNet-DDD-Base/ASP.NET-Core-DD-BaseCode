using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using UserApp.Infrastructure;
using UserApp.Infrastructure.Persistence;
using UserApp.Web.Mapping;
using UserApp.Infrastructure.Persistence.Repositories;
using UserApp.Domain.Common;
using UserApp.Domain.Users;
using UserApp.Application.Common;
using UserApp.Application.Users;
using UserApp.Application.Users.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserApp.Application.Common.Interfaces;
using UserApp.Infrastructure.Media;
using UserApp.Domain.Media;
using UserApp.Domain.Roles;
using System.Security.Claims;
using UserApp.Infrastructure.Security;
using UserApp.Web.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using UserApp.Application.Roles.Interfaces;
using UserApp.Application.Roles;
using UserApp.Application.Permissions.Interfaces;
using UserApp.Application.Permissions;
using UserApp.Domain.Categorys;
using UserApp.Application.Categorys;
using UserApp.Application.Categorys.Interfaces;
using UserApp.Domain.CommonTables;
using UserApp.Application.CommonTables;
using UserApp.Application.CommonTables.Interfaces;
using UserApp.Domain.SidebarItems;
using UserApp.Application.SidebarItems;
using UserApp.Application.SidebarItems.Interfaces;
using UserApp.Domain.SidebarGroups;
using UserApp.Application.SidebarGroups;
using UserApp.Application.SidebarGroups.Interfaces;
using UserApp.Domain.Customers;
using UserApp.Application.Customers;
using UserApp.Application.Customers.Interfaces;
using UserApp.Domain.Orders;
using UserApp.Application.Orders;
using UserApp.Application.Orders.Interfaces;
using UserApp.Domain.OrderDetails;
using UserApp.Application.OrderDetails;
using UserApp.Application.OrderDetails.Interfaces;
// ================= AUTO MODULE IMPORTS =================
// <AUTO-USINGS-START>
// <AUTO-USINGS-END>
using UserApp.Domain.AuditLogs;
using UserApp.Application.AuditLogs;
using UserApp.Application.AuditLogs.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------
// Infrastructure (DbContext)
// ------------------------------------------------
builder.Services.AddInfrastructure(builder.Configuration);

// ------------------------------------------------
// AutoMapper
// ------------------------------------------------
var mapperConfiguration = new MapperConfiguration(
    cfg =>
    {
        cfg.AddProfile<MappingProfile>();
    },
    NullLoggerFactory.Instance);

builder.Services.AddSingleton(mapperConfiguration.CreateMapper());

// ------------------------------------------------
// REPOSITORIES
// ------------------------------------------------
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// ================= AUTO REPOSITORIES =================
// <AUTO-REPOSITORIES-START>
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommonTableRepository, CommonTableRepository>();
builder.Services.AddScoped<ISidebarItemRepository, SidebarItemRepository>();
builder.Services.AddScoped<ISidebarGroupRepository, SidebarGroupRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
// <AUTO-REPOSITORIES-END>
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();


// ------------------------------------------------
// SERVICES (Application Layer)
// ------------------------------------------------
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

// ================= AUTO SERVICES =================
// <AUTO-SERVICES-START>
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommonTableService, CommonTableService>();
builder.Services.AddScoped<ISidebarItemService, SidebarItemService>();
builder.Services.AddScoped<ISidebarGroupService, SidebarGroupService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
// <AUTO-SERVICES-END>

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<
    UserApp.Application.Common.Interfaces.IModuleGeneratorService,
    UserApp.Infrastructure.Services.ModuleGeneratorService>();
builder.Services.AddScoped<MediaStorage>();
builder.Services.AddScoped<IMediaPipeline, MediaPipeline>();


builder.Services.AddScoped<PermissionFilter>();
builder.Services.AddScoped<AuditContextActionFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<PermissionFilter>();
    options.Filters.AddService<AuditContextActionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "THIS_IS_DEMO_SECRET_KEY_123456";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(
                    """{"success":false,"message":"Unauthorized"}"""
                );
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(
                    """{"success":false,"message":"Forbidden"}"""
                );
            }
        };
    });

builder.Services.AddAuthorization();

// 1. ADD CORS POLICY HERE 
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173") // Your Vite Dev Server
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Sync migration history: mark auto-generated migrations as applied if their table already exists
    await SyncMigrationHistoryAsync(db);

    await db.Database.MigrateAsync();
    await UserApp.Infrastructure.Persistence.Seed.RbacSeeder.SeedRolesAsync(db);
    await UserApp.Infrastructure.Persistence.Seed.RbacSeeder.SeedPermissionsAsync(db);
    await UserApp.Infrastructure.Persistence.Seed.RbacSeeder.SeedAdminRolePermissionsAsync(db);
    await SeedFlashMessages(db);
    await SeedSidebarGroups(db);
    await SeedSidebarItems(db);

    // Create FK if it doesn't exist (must be after SidebarGroups are seeded)
    await db.Database.ExecuteSqlRawAsync(@"
        SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'SidebarItems' AND CONSTRAINT_NAME = 'FK_SidebarItems_SidebarGroups_GroupId');
        SET @stmt = IF(@fk_exists = 0,
            'ALTER TABLE SidebarItems ADD CONSTRAINT FK_SidebarItems_SidebarGroups_GroupId FOREIGN KEY (GroupId) REFERENCES SidebarGroups(Id) ON DELETE CASCADE',
            'SELECT 1'
        );
        PREPARE s FROM @stmt;
        EXECUTE s;
        DEALLOCATE PREPARE s;
    ");
}

static async Task SyncMigrationHistoryAsync(AppDbContext db)
{
    // Sync auto-generated table migrations (create if table already exists)
    try
    {
        await db.Database.ExecuteSqlRawAsync(@"
        ");
    }
    catch { }

    // Mark DropCategoryPriceAndDescription as applied (no-op if columns already adjusted)
    try
    {
        var applied = (await db.Database.GetAppliedMigrationsAsync()).ToHashSet();
        if (!applied.Contains("20260617163210_DropCategoryPriceAndDescription"))
        {
            await db.Database.ExecuteSqlRawAsync(
                "INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('20260617163210_DropCategoryPriceAndDescription', '8.0.0')");
        }
    }
    catch { }
}

static async Task SeedSidebarGroups(AppDbContext db)
{
    var existing = db.Set<UserApp.Domain.SidebarGroups.SidebarGroup>()
        .Select(x => x.Name)
        .ToHashSet();

    var groups = new (string name, int order)[]
    {
        ("Master Data", 1),
        ("Commerce", 2),
        ("Operations", 3),
        ("Communication", 4),
        ("AI", 5),
        ("System", 6),
    };

    foreach (var (name, order) in groups)
    {
        if (existing.Contains(name)) continue;

        db.Set<UserApp.Domain.SidebarGroups.SidebarGroup>().Add(new()
        {
            Name = name,
            DisplayOrder = order,
            IsActive = true
        });
    }

    await db.SaveChangesAsync();
}

static async Task SeedSidebarItems(AppDbContext db)
{
    var existing = db.Set<UserApp.Domain.SidebarItems.SidebarItem>()
        .Select(x => x.ControllerName)
        .ToHashSet();

    var groupMap = db.Set<UserApp.Domain.SidebarGroups.SidebarGroup>()
        .ToDictionary(x => x.Name, x => x.Id);

    var items = new (string moduleName, string controllerName, string groupName, int order)[]
    {
        ("Common Tables", "CommonTable", "Master Data", 1),
        ("Categories",    "Category",    "Master Data", 2),
        ("Media",         "Media",       "Communication", 2),
        ("Users",         "Users",       "System",      1),
        ("Roles",         "Roles",       "System",      2),
        ("Permissions",   "Permissions", "System",      3),
        ("Module Generator", "ModuleGenerator", "System", 4),
        ("Audit Log", "AuditLog", "System", 5),
    };

    foreach (var (moduleName, controllerName, groupName, order) in items)
    {
        if (existing.Contains(controllerName)) continue;

        if (!groupMap.TryGetValue(groupName, out var groupId))
            groupId = Guid.Empty;

        db.Set<UserApp.Domain.SidebarItems.SidebarItem>().Add(new()
        {
            ModuleName = moduleName,
            ControllerName = controllerName,
            GroupId = groupId,
            DisplayOrder = order,
            IsActive = true
        });
    }

    await db.SaveChangesAsync();
}

static async Task SeedFlashMessages(AppDbContext db)
{
    var existing = db.Set<UserApp.Domain.CommonTables.CommonTable>()
        .Where(x => x.Type == "FlashMessage")
        .Select(x => x.Code)
        .ToHashSet();

    var modules = new[] { "Category" };
    var actions = new[] { "Create", "Edit", "Delete" };

    foreach (var module in modules)
    {
        foreach (var action in actions)
        {
            var code = $"{module}{action}";
            if (existing.Contains(code)) continue;

            db.Set<UserApp.Domain.CommonTables.CommonTable>().Add(new()
            {
                Type = "FlashMessage",
                Code = code,
                Name = $"{module} {action.ToLower()} successfully"
            });
        }
    }

    await db.SaveChangesAsync();
}

await app.InitializeDatabaseAsync();

// ... (database migrations and environments check)

app.Use(async (context, next) =>
{
    UserApp.Application.Common.ServiceProviderAccessor.Current = context.RequestServices;
    await next();
});

app.UseStaticFiles();

app.UseRouting();


// 2. 🔥 ACTIVATE CORS MIDDLEWARE (MUST BE PLACED IN THIS EXACT ORDER)
app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------
// ROUTES
// ------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.Run();

