using UserApp.Application.Common;
using UserApp.Domain.Products;
using UserApp.Application.Products.Interfaces;

namespace UserApp.Application.Products;

public class ProductService : BaseService<Product>, IProductService
{
    public ProductService(IProductRepository repo) : base(repo)
    {
    }
}
