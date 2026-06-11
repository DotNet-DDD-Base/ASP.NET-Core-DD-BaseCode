using Microsoft.EntityFrameworkCore;
using UserApp.Domain.Users;
using UserApp.Infrastructure.Persistence.Configurations;
using MediaEntity = UserApp.Domain.Media.MediaFile;
using UserApp.Domain.Paps;
using UserApp.Domain.Milks;
using UserApp.Domain.Ais;
using UserApp.Domain.Cocos;
using UserApp.Domain.Roles;


namespace UserApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ==================== DBSets ====================
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MediaEntity> Media => Set<MediaEntity>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    // ================= AUTO DBSets =================
    // <AUTO-DBSETS-START>
    public DbSet<Pap> Paps => Set<Pap>();
    public DbSet<Milk> Milks => Set<Milk>();
    public DbSet<Ai> Ais => Set<Ai>();
    public DbSet<Coco> Cocos => Set<Coco>();
    // <AUTO-DBSETS-END>



    // NEW MODULE (Payment)
    // ==================== MODEL CONFIG ====================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());

        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);
    }
}
