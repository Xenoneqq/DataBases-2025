using Microsoft.EntityFrameworkCore;

using var dbContext = new DatabaseContext();

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();

var supplier1 = new Supplier
{
    CompanyName = "Delta Logistics",
    Street = "ul. Transportowa 12",
    City = "Poznań",
    ZipCode = "60-001",
    BankAccountNumber = "PL61 1090 1014 0000 0712 1981 2874"
};
var supplier2 = new Supplier
{
    CompanyName = "TechFusion",
    Street = "ul. Nowowiejska 5",
    City = "Kraków",
    ZipCode = "31-000",
    BankAccountNumber = "PL61 2345 6789 0000 0123 4567 8901"
};
dbContext.Suppliers.AddRange(supplier1, supplier2);
dbContext.SaveChanges();

var customer1 = new Customer
{
    CompanyName = "Sklep Zabawki",
    Street = "ul. Dziecięca 3",
    City = "Wrocław",
    ZipCode = "50-001",
    Discount = 0.12m
};
var customer2 = new Customer
{
    CompanyName = "SuperMarket S.A.",
    Street = "ul. Ogrodowa 7",
    City = "Gdańsk",
    ZipCode = "80-001",
    Discount = 0.05m
};
dbContext.Customers.AddRange(customer1, customer2);
dbContext.SaveChanges();

// supplierzy
Console.WriteLine("Supplier:");
var dostawcy = dbContext.Suppliers
    .AsNoTracking()
    .OrderBy(s => s.CompanyID)
    .ToList();
foreach (var d in dostawcy)
{
    Console.WriteLine($"- SupplierID={d.CompanyID}, Nazwa={d.CompanyName}, Bank={d.BankAccountNumber}");
}

Console.WriteLine();

// customersi
Console.WriteLine("Customer:");
var klienci = dbContext.Customers
    .AsNoTracking()
    .OrderBy(c => c.CompanyID)
    .ToList();
foreach (var k in klienci)
{
    Console.WriteLine($"- CustomerID={k.CompanyID}, Nazwa={k.CompanyName}, Discount={k.Discount:P0}");
}