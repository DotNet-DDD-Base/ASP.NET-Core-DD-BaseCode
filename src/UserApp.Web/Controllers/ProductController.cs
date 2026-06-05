using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Domain.Products;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class ProductsController 
    : BaseController<Product, ProductViewModel>
{
    public ProductsController(IProductRepository repo, IMapper mapper)
        : base(repo, mapper)
    {
    }
}