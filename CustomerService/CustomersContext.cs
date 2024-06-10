
using CustomersDemo;
using Microsoft.EntityFrameworkCore;


namespace CustomersDemo;

public sealed class CustomersContext:DbContext
{
  public DbSet<Customer> Customers { get; set; }

 public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
  {
  }
}