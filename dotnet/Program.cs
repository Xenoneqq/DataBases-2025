// See https://aka.ms/new-console-template for more information

using System;
ProdContext prodContext = new ProdContext();
Product product = new Product { ProductName = "Flamaster"};
prodContext.Products.Add(product);
prodContext.SaveChanges();

public class Product {
    public int ProductID {get; set;}
    public String? ProductName {get; set;} 
    public int UnitsInStock {get; set;}
}




