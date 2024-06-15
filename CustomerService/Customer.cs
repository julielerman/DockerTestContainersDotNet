namespace CustomersDemo;

public class Customer
{
  public int Id { get; private set;}
  public string Name { get; private set;}
  public Customer (string name)
  {
    Name=name;
  }
  
}