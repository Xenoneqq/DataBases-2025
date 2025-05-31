using Microsoft.EntityFrameworkCore;

using DatabaseContext dbContext = new DatabaseContext();

dbContext.Suppliers.Add(
    new Supplier()
    {
        CompanyName = "ACME Corp"
    }
);

var product = dbContext.Products
                           .Include(p => p.Supplier)
                           .SingleOrDefault(p => p.ProductName == "Kaczka");

var supplier = dbContext.Suppliers.SingleOrDefault(s => s.CompanyName == "ACME Corp");

if (product != null)
    product.Supplier = supplier;

dbContext.SaveChanges();

var query = dbContext.Products
                     .Include(p => p.Supplier)
                     .Select(p => new 
                     {
                         ProductName = p.ProductName,
                         SupplierName = p.Supplier.CompanyName
                     });

foreach (var item in query)
{
    Console.WriteLine($"{item.ProductName}, Dostawca: {item.SupplierName}");
}

var query2 = from sup in dbContext.Suppliers
select sup.CompanyName;
Console.WriteLine("\nCompanies:");
foreach (var sup in query2)
{
    Console.WriteLine(sup);
}
