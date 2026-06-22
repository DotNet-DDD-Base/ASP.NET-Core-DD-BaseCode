using Xunit;
using UserApp.Domain.Customers;

namespace UserApp.Tests.Domain.Customers;

public class CustomerTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var entity = new Customer();

        Assert.Equal(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt > DateTime.MinValue);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.DeletedAt);
        Assert.False(entity.IsDeleted);
    }

    [Fact]
    public void Inherits_EntityGuid()
    {
        var entity = new Customer();
        Assert.IsAssignableFrom<UserApp.Domain.Common.Entity<Guid>>(entity);
    }

    [Fact]
    public void Implements_IHasMedia()
    {
        var entity = new Customer();
        Assert.IsAssignableFrom<UserApp.Domain.Common.IHasMedia>(entity);
    }

    [Fact]
    public void Delete_SetsDeletedAt()
    {
        var entity = new Customer();
        entity.Delete();

        Assert.NotNull(entity.DeletedAt);
        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public void Restore_ClearsDeletedAt()
    {
        var entity = new Customer();
        entity.Delete();
        entity.Restore();

        Assert.Null(entity.DeletedAt);
        Assert.False(entity.IsDeleted);
    }
}
