using CustomersDemo;

namespace Customers;

public sealed class CustomerService
{
    private readonly CustomersContext _context;

    public CustomerService(CustomersContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    public IEnumerable<Customer> GetCustomers()
    {
        return  _context.Customers.ToList();;
    }

    public void Create(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

   
}
