// See https://aka.ms/new-console-template for more information

using System;
using System.Dynamic;
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

query = from sup in dbContext.Suppliers
select sup.CompanyName;

foreach (var sup in query)
{
    Console.WriteLine(sup);
}

public class Product {
    public int ProductID {get; set;}
    public String? ProductName {get; set;} 
    public int UnitsInStock {get; set;}

    public int SupplierID {get; set;}
    public Supplier Supplier {get; set;}
}


public class Supplier {
    public int SupplierID {get; set;}
    public String? CompanyName {get; set;}
    public String? Street {get; set;} 
    public String? City {get; set;}

    public ICollection<Product> Products {get; set;}
}