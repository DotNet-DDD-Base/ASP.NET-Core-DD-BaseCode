using AutoMapper;
using UserApp.Domain.Users;
using UserApp.Web.ViewModels;
using UserApp.Application.Users.DTOs;
using UserApp.Web.ViewModels.Roles;
using UserApp.Domain.Roles;
using UserApp.Web.ViewModels.Permissions;
using UserApp.Domain.Categorys;
using UserApp.Domain.CommonTables;
using UserApp.Web.ViewModels.CommonTables;

using UserApp.Domain.Customers;
using UserApp.Domain.Orders;
using UserApp.Domain.OrderDetails;
using UserApp.Domain.AuditLogs;
using UserApp.Web.ViewModels.AuditLogs;



namespace UserApp.Web.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ==================== USER MAPPINGS ====================
        CreateMap<User, UserDto>();
        CreateMap<User, UserViewModel>();

        CreateMap<Role, RoleViewModel>()
            .ReverseMap();

        CreateMap<Permission, PermissionViewModel>()
                    .ReverseMap();

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UserViewModel, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());


        // ================= AUTO MAPPINGS =================
        // <AUTO-MAPPINGS-START>

        CreateMap<CommonTable, CommonTableViewModel>();
        CreateMap<CommonTableViewModel, CommonTable>();
CreateMap<Category, CategoryViewModel>();
CreateMap<CategoryViewModel, Category>();
CreateMap<Customer, CustomerViewModel>();
CreateMap<CustomerViewModel, Customer>();
CreateMap<Order, OrderViewModel>();
CreateMap<OrderViewModel, Order>();
CreateMap<OrderDetail, OrderDetailViewModel>();
CreateMap<OrderDetailViewModel, OrderDetail>();
        CreateMap<AuditLog, AuditLogViewModel>();
        // <AUTO-MAPPINGS-END>
        CreateMap<RoleViewModel, Role>();
        CreateMap<PermissionViewModel, Permission>();



    }
}
