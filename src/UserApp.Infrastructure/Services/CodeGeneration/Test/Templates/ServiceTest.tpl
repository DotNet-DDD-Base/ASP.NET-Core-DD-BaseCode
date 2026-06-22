using System.Reflection;
using Moq;
using Xunit;
using UserApp.Application.{{Name}}s;
using UserApp.Application.{{Name}}s.Interfaces;
using UserApp.Domain.Common;
using UserApp.Domain.{{Name}}s;

namespace UserApp.Tests.Application.{{Name}}s;

public class {{Name}}ServiceTests
{
    private readonly Mock<I{{Name}}Repository> _repoMock;
    private readonly I{{Name}}Service _service;

    public {{Name}}ServiceTests()
    {
        _repoMock = new Mock<I{{Name}}Repository>();
        _service = new {{Name}}Service(_repoMock.Object);
    }

    private static void SetId({{Name}} entity, Guid id)
    {
        typeof(Entity<Guid>).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(entity, id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenFound()
    {
        var id = Guid.NewGuid();
        var entity = new {{Name}}();
        SetId(entity, id);
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(({{Name}}?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_DelegatesToRepository()
    {
        var items = new List<{{Name}}> { new(), new() };
        _repoMock.Setup(r => r.ListAsync(0, 10)).ReturnsAsync(items);

        var result = await _service.ListAsync(0, 10);

        Assert.Equal(2, result.Count);
        _repoMock.Verify(r => r.ListAsync(0, 10), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        _repoMock.Setup(r => r.CountAsync()).ReturnsAsync(5);

        var result = await _service.CountAsync();

        Assert.Equal(5, result);
        _repoMock.Verify(r => r.CountAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_CallsAddAndSaveChanges()
    {
        var entity = new {{Name}}();

        await _service.AddAsync(entity);

        _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentNullException_WhenEntityNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddAsync(null!));
        _repoMock.Verify(r => r.AddAsync(It.IsAny<{{Name}}>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_CallsUpdateAndSaveChanges()
    {
        var entity = new {{Name}}();

        await _service.UpdateAsync(entity);

        _repoMock.Verify(r => r.Update(entity), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenEntityNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
    }

    [Fact]
    public async Task RemoveAsync_CallsRemoveAndSaveChanges()
    {
        var entity = new {{Name}}();

        await _service.RemoveAsync(entity);

        _repoMock.Verify(r => r.Remove(entity), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ThrowsArgumentNullException_WhenEntityNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemoveAsync(null!));
    }

    [Fact]
    public async Task SaveAsync_CallsSaveChanges()
    {
        await _service.SaveAsync();
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
