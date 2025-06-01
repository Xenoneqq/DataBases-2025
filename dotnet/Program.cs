using Microsoft.EntityFrameworkCore;

using DatabaseContext dbContext = new DatabaseContext();

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();

var supplier1 = new Supplier
{
    CompanyName = "Delta Logistics",
    Street = "ul. Transportowa 12",
    City = "Poznań",
    ZipCode = "60-001",
    BankAccountNumber = "PL69 1090 1014 0000 2137 1981 2874"
};

var supplier2 = new Supplier
{
    CompanyName = "TechFusion",
    Street = "ul. Nowowiejska 5",
    City = "Kraków",
    ZipCode = "31-000",
    BankAccountNumber = "PL61 2345 6789 0420 0123 4567 8901"
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

var product1 = new Product
{
    ProductName = "Rower Miejski",
    UnitsOnStock = 15,
    SupplierID = supplier1.CompanyID   
};
var product2 = new Product
{
    ProductName = "Hulajnoga Elektryczna",
    UnitsOnStock = 8,
    SupplierID = supplier1.CompanyID
};

dbContext.Products.AddRange(product1, product2);
dbContext.SaveChanges();

var invoice = new Invoice
{
    InvoiceNumber = "F2025-100",
    InvoiceDate = DateTime.Today
};
dbContext.Invoices.Add(invoice);
dbContext.SaveChanges();

var ip1 = new InvoiceProduct
{
    InvoiceID = invoice.InvoiceID,
    ProductID = product1.ProductID,
    Quantity = 3
};
var ip2 = new InvoiceProduct
{
    InvoiceID = invoice.InvoiceID,
    ProductID = product2.ProductID,
    Quantity = 1
};
dbContext.InvoiceProducts.AddRange(ip1, ip2);
dbContext.SaveChanges();

Console.WriteLine("Wszystkie firmy w bazie:");
var wszystkieFirmy = dbContext.Companies
    .AsNoTracking()
    .OrderBy(c => c.CompanyID)
    .ToList();

foreach (var firma in wszystkieFirmy)
{
    // Sprawdzamy typ po rzutowaniu
    if (firma is Supplier s)
    {
        Console.WriteLine($"[Supplier]  ID = {s.CompanyID}, Nazwa = {s.CompanyName}, Miasto = {s.City}, BankAccount = {s.BankAccountNumber}");
    }
    else if (firma is Customer c)
    {
        Console.WriteLine($"[Customer]  ID = {c.CompanyID}, Nazwa = {c.CompanyName}, Miasto = {c.City}, Discount = {c.Discount:P0}");
    }
    else
    {
        Console.WriteLine($"[???]       ID = {firma.CompanyID}, Nazwa = {firma.CompanyName}");
    }
}

Console.WriteLine();

// tylko suppliers
Console.WriteLine("Wyłącznie dostawcy:");
var dostawcy = dbContext.Suppliers
    .AsNoTracking()
    .OrderBy(s => s.CompanyID)
    .ToList();

foreach (var d in dostawcy)
{
    Console.WriteLine($"- SupplierID = {d.CompanyID}, Nazwa = {d.CompanyName
                        }, Bank = {d.BankAccountNumber}, Miasto = {d.City}");
}

Console.WriteLine();

// tylko klienci
Console.WriteLine("Wyłącznie klienci:");
var klienci = dbContext.Customers
    .AsNoTracking()
    .OrderBy(c => c.CompanyID)
    .ToList();

foreach (var k in klienci)
{
    Console.WriteLine($"- CustomerID = {k.CompanyID}, Nazwa = {k.CompanyName
                        }, Discount = {k.Discount:P0}, Miasto = {k.City}");
}
