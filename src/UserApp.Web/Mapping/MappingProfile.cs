using AutoMapper;
using UserApp.Domain.Users;
using UserApp.Domain.Products;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserViewModel>().ReverseMap();

        CreateMap<Product, ProductViewModel>().ReverseMap();
    }
}