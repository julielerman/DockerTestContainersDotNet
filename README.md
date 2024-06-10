This project is a twist on the Getting Started [doc] (https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet) for dotnet developers on the TestContainers website.

Rather than using raw code to interact with Postgre, this solution takes advantage of EF Core.

There are two branches.

The main branch has a Customer Service project. You can add to this by creating a test project that uses an npgsql Test container to run the tests without having to be concerend with accessing the image or starting a container.

The IncludingTests branch includes the already built test project which you can test or use to compare to your own code.

Instructions to add the test project manually and get a feel for how to add a test container in on your own:

Create a new test project (I used XUnit) in the solution called CustomerService.Tests. If you are using the CLI, the command is
If you are working at the command line you can use the following commands:

```
dotnet new xunit -o CustomerService.Tests
dotnet sln add ./CustomerService.Tests/CustomerService.Tests.csproj  
dotnet add ./CustomerService.Tests/CustomerService.Tests.csproj reference ./CustomerService/CustomerService.csproj
```

The key to the test container is to have a package reference to it in the csproj file:

```
<PackageReference Include="Testcontainers.PostgreSql" Version="3.3.0" />
```

With that you can use it in your test code. In the unit test file, you'll create an object from the testcontainer.

Start with using statements in your test class and your class should implement IAsyncLifetime.

```
using CustomersDemo;  //points to the other project
using Testcontainers.PostgreSql; //points to the container
using Microsoft.EntityFrameworkCore;

public sealed class CustomerServiceTest : IAsyncLifetime
{
}

```

Define the container object as a variable in the class:

```
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

```

The container will need to be started and taken down during the test. XUnit will call InitializeAsync and DisposeAsync internally so you can override those methods so they start and stop the container.

```
  public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
```

Finally, we have the test! It will grab the default connection string from the container (_postgres variable) and use that to pass in the needed db options to the EF Core CustomersContext defined in the service project.

Then it creates two new customers using methods in the service which also saves them to the database.

Next it retrieves all customers from the database and asserts that there are two.

```
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
```
