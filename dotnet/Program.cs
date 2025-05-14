// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;

ProdContext prodContext = new ProdContext();
Console.WriteLine("Podaj nazwę produktu: ");
String? prodName = Console.ReadLine();
Product product = new Product { ProductName = prodName};
prodContext.Products.Add(product);
prodContext.SaveChanges();
var query = from prod in prodContext.Products
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

