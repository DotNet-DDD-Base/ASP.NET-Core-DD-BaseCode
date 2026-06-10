using UserApp.Domain.Common;
using UserApp.Application.Common.Interfaces;


namespace UserApp.Application.Common;

public class BaseService<T> : IBaseService<T> where T : class
{
    protected readonly IBaseRepository<T> _repo;
    protected readonly IMediaPipeline? _mediaPipeline;
    protected readonly IMediaService? _mediaService;

    public BaseService(IBaseRepository<T> repo)
    {
        _repo = repo;
        _mediaPipeline = ServiceProviderAccessor.Current?.GetService(typeof(IMediaPipeline)) as IMediaPipeline;
        _mediaService = ServiceProviderAccessor.Current?.GetService(typeof(IMediaService)) as IMediaService;
    }

    public Task<T?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
    public Task<List<T>> ListAsync(int skip, int take) => _repo.ListAsync(skip, take);
    public Task<int> CountAsync() => _repo.CountAsync();

    public async Task AddAsync(T entity, object? file = null)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _repo.AddAsync(entity);

        // 🔥 FIRST SAVE so ID exists
        await _repo.SaveChangesAsync();

        // THEN media only for entities that explicitly support media
        if (entity is IHasMedia && _mediaPipeline != null && file != null)
        {
            await _mediaPipeline.HandleCreateAsync(typeof(T).Name, entity, file);
            // 🔥 SAVE MEDIA
            await _repo.SaveChangesAsync();
        }


    }

    public async Task UpdateAsync(T entity, object? file = null)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _repo.Update(entity);

        // 🔥 HANDLE MEDIA FIRST (track changes in same context)
        if (entity is IHasMedia && _mediaPipeline != null && file != null)
        {
            await _mediaPipeline.HandleUpdateAsync(typeof(T).Name, entity, file);
        }

        // 🔥 SAVE EVERYTHING TOGETHER
        await _repo.SaveChangesAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (entity is IHasMedia && _mediaPipeline != null)
        {
            await _mediaPipeline.HandleDeleteAsync(typeof(T).Name, entity);
        }

        _repo.Remove(entity);
        await _repo.SaveChangesAsync();
    }

    public virtual async Task<List<string>> GetImageUrlsAsync(Guid id)
    {
        if (_mediaService == null)
            return [];

        var media = await _mediaService.GetAsync(typeof(T).Name, id);
        return media.Select(x => x.Url).ToList();
    }

    public Task SaveAsync() => _repo.SaveChangesAsync();
}