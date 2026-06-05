using UserApp.Domain.Common;

namespace UserApp.Application.Common;

public class BaseService<T> where T : class
{
    private readonly IBaseRepository<T> _repo;

    public BaseService(IBaseRepository<T> repo)
    {
        _repo = repo;
    }

    public Task<T?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public Task<List<T>> ListAsync(int skip, int take) => _repo.ListAsync(skip, take);

    public Task AddAsync(T entity)
        => _repo.AddAsync(entity);

    public void Update(T entity)
        => _repo.Update(entity);

    public void Remove(T entity)
        => _repo.Remove(entity);

    public Task SaveAsync()
        => _repo.SaveChangesAsync();
}