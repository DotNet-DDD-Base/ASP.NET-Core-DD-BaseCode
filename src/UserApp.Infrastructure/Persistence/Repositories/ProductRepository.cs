using UserApp.Domain.Products;

namespace UserApp.Infrastructure.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext db) : base(db)
    {
    }
}