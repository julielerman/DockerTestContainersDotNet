This project is a twist on the Getting Started [doc] (https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet) for dotnet developers on the TestContainers website.

## TLDR TestContainers ##
With a simple reference to a particular TestContainer package (in this demo it is for PostgreSQL), you can create an object in code to simply use what is exposed in the container.

For example, in this solution, the test project csproj file has a reference to the Testcontainers.PostgreSql package. In the test, you'll use its API to build a local image  ...but it's just another object in your logic.

```c#
private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();
```

Then you can interact with the object as you will see in the test project code described below. In this case that will be starting and stopping the container, reading its connection string and interacting with the database.

Rather than using raw ADO.NET to interact with PostgreSQL as the original solution, my version uses EF Core.

## The two branches ##

### main branch ###
The main branch has a Customer Service project. You can add to this by creating a test project (instructions below) that uses an npgsql Test container to run the tests without having to be concerend with accessing the image or starting a container.

The project has a Customer record type defined in a customers.cs file, an EF Core DbContext called CustomersContext and a CustomerService class with methods to add a customer and retrieve all customers from the database using EF Core.

### IncludingTests branch ###
The IncludingTests branch includes the already built test project which you can test or use to compare to your own code.

## Adding the test project to the main solution ##
This will help you to get a feel for how to add a test container on your own:

* Create a new test project (I used XUnit) in the solution called CustomerService.Tests. If you are using the CLI, the command is
If you are working at the command line you can use the following commands:

```c#
dotnet new xunit -o CustomerService.Tests
dotnet sln add ./CustomerService.Tests/CustomerService.Tests.csproj  
dotnet add ./CustomerService.Tests/CustomerService.Tests.csproj reference ./CustomerService/CustomerService.csproj
```

* The key to the test container is to have a package reference to it in the csproj file. Use your favorite method of getting that into the file.

```c#
<PackageReference Include="Testcontainers.PostgreSql" Version="3.3.0" />
```
With that, you can access the TestContainer  in your test code. You'll create an object from the testcontainer.

* Start with using statements in your test class and your class should implement IAsyncLifetime.

```c#
using CustomersDemo;  //points to the other project
using Testcontainers.PostgreSql; //points to the container
using Microsoft.EntityFrameworkCore;

public sealed class CustomerServiceTest : IAsyncLifetime
{
}

```

* Define the container object as a variable in the class:

```c#
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

```

* The container will need to be started and taken down during the test. XUnit will call InitializeAsync and DisposeAsync internally. Override those methods in the class so they will start and stop the container.

```c#
  public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
```

Finally, we have the test, itself! The following code will grab the default connection string from the container (_postgres variable) and use that to pass in the needed db options to the EF Core CustomersContext defined in the service project.

Then it creates two new customers using methods in the service which also saves them to the database.

Next it retrieves all customers from the database and asserts that there are two.

```c#
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
## Time to Test ##

There are many ways to test and they are dependent on your personal pref.  

If you are using CLI, then be sure you are in the test folder and run `dotnet test` 

You can also of course use all of the fancy testing tools in Visual Studio and other IDEs.
