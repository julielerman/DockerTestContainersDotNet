See the [readme for the main branch](https://github.com/julielerman/DockerTestContainersDotNet/tree/main). :grin:

This branch has shifted to use SQL Server. The customer class is now designed to presume a generated Id. And the package referenced for EF is now the SQL Server provider.

The test project references the MsSql test container in csproj and the test is refactored. It spins up an MsSqlTest that you would otherwise build in the main branch.
  
 ```
  private readonly MsSqlContainer _mssql = 
   new MsSqlBuilder()
   .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
   .Build();
   ```
   Keep in mind that this is just to demonstrate the basic usage of the MsSql testcontainer. It is not set up to demo best practices. Starting up the container just for one simple test is an abuse with respect to performance. The Postgre test takes about 7 seconds (most of which is for the container) where this one takes closer to 30.

   Also, when you are doing integration tests, you need to be selective about which tests require working with the true database and justify the perf cost. (And here is a shameless plug for my EF Core 8 Fundamentals course on Pluralsight that has a complete module on testing. :grin: )

   Check [this reddit thread](https://www.reddit.com/r/dotnet/comments/16j65bf/best_practices_with_net_core_and/) for some direction on reusing testcontainers including those taht have seed data or even a replica of more significant test data.

