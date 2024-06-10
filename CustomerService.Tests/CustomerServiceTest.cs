using Testcontainers.PostgreSql;
using CustomersDemo;
using Microsoft.EntityFrameworkCore;
namespace Customers.Tests;

public sealed class CustomerServiceTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();
        
   
    public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public void ShouldReturnTwoCustomers()
    {
        var builder = new DbContextOptionsBuilder<CustomersContext>()
    .UseNpgsql(_postgres.GetConnectionString());

        // Given
        var customerService = 
          new CustomerService(new CustomersContext(builder.Options));

        // When
        customerService.Create(new Customer(1, "George"));
        customerService.Create(new Customer(2, "John"));
        var customers = customerService.GetCustomers();

        // Then
        Assert.Equal(2, customers.Count());
    }
}