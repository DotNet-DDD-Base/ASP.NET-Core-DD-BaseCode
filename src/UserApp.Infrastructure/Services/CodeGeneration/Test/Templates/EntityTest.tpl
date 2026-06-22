using Xunit;
using UserApp.Domain.{{Name}}s;

namespace UserApp.Tests.Domain.{{Name}}s;

public class {{Name}}Tests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var entity = new {{Name}}();

        Assert.Equal(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt > DateTime.MinValue);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.DeletedAt);
        Assert.False(entity.IsDeleted);
    }

    [Fact]
    public void Inherits_EntityGuid()
    {
        var entity = new {{Name}}();
        Assert.IsAssignableFrom<UserApp.Domain.Common.Entity<Guid>>(entity);
    }
{{HasImageTest}}

    [Fact]
    public void Delete_SetsDeletedAt()
    {
        var entity = new {{Name}}();
        entity.Delete();

        Assert.NotNull(entity.DeletedAt);
        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public void Restore_ClearsDeletedAt()
    {
        var entity = new {{Name}}();
        entity.Delete();
        entity.Restore();

        Assert.Null(entity.DeletedAt);
        Assert.False(entity.IsDeleted);
    }
}
