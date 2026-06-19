using UserApp.Application.Common;
using UserApp.Domain.Customers;
using UserApp.Application.Customers.Interfaces;

namespace UserApp.Application.Customers;

public class CustomerService : BaseService<Customer>, ICustomerService
{
    public CustomerService(ICustomerRepository repo) : base(repo)
    {
    }
}
