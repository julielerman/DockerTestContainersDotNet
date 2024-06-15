using Testcontainers.MsSql;
using CustomersDemo;
using Microsoft.EntityFrameworkCore;
namespace Customers.Tests;

public sealed class CustomerServiceTest : IAsyncLifetime
{
   private readonly MsSqlContainer _mssql = 
   new MsSqlBuilder()
   .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
   .Build();
        
    public Task InitializeAsync()
    {
        return _mssql.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mssql.DisposeAsync().AsTask();
    }

    [Fact]
    public void ShouldReturnTwoCustomers()
    {
        var builder = new DbContextOptionsBuilder<CustomersContext>()
    .UseSqlServer(_mssql.GetConnectionString());
     var context=new CustomersContext(builder.Options);
     context.Database.EnsureCreated();
        // Given
        var customerService = 
          new CustomerService(context);

        // When
        customerService.Create(new Customer("George"));
        customerService.Create(new Customer( "John"));
        var customers = customerService.GetCustomers();

        // Then
        Assert.Equal(2, customers.Count());
    }
}