using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserApp.Application.Customers.Interfaces;
using UserApp.Domain.Customers;
using UserApp.Web.ViewModels;

namespace UserApp.Web.Controllers;

public class CustomerController : BaseController<Customer, CustomerViewModel>
{
    public CustomerController(ICustomerService service, IMapper mapper) : base(service, mapper)
    {
    }
}
