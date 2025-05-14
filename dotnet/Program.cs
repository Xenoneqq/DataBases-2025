// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;

DatabaseContext dbContext = new DatabaseContext();
Console.WriteLine("Podaj nazwę produktu: ");
String? prodName = Console.ReadLine();
Product product = new Product { ProductName = prodName};
dbContext.Products.Add(product);
dbContext.SaveChanges();
var query = from prod in dbContext.Products
select prod.ProductName;

foreach (var pName in query)
{
    Console.WriteLine(pName);
}



public class Product {
    public int ProductID {get; set;}
    public String? ProductName {get; set;} 
    public int UnitsInStock {get; set;}
}


public class Supplier {
    public int SupplierID {get; set;}
    public String? CompanyName {get; set;}
    public String? Street {get; set;} 
    public String? City {get; set;}
}