using Microsoft.EntityFrameworkCore;
using UserApp.Infrastructure;
using UserApp.Infrastructure.Persistence;
using UserApp.Web.Mapping;
using UserApp.Infrastructure.Persistence.Repositories;
using UserApp.Domain.Common;
using UserApp.Domain.Users;
using UserApp.Domain.Products;


var builder = WebApplication.CreateBuilder(args);

// Add MVC with default Razor conventions
builder.Services.AddControllersWithViews();

// Add Infrastructure (DbContext, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register generic repositories
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Register domain-specific repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Apply database migrations on startup (for dev/testing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();