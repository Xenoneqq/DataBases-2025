using Microsoft.EntityFrameworkCore;

using DatabaseContext dbContext = new DatabaseContext();

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();

var supplier = new Supplier
{
    CompanyName = "Myarse Company",
    Street = "Ulica Przykładowa 5",
    City = "Warszawa"
};
dbContext.Suppliers.Add(supplier);
dbContext.SaveChanges();

var produkt1 = new Product { ProductName = "Rower", UnitsOnStock = 10, SupplierID = 1 };
var produkt2 = new Product { ProductName = "Auto", UnitsOnStock = 5, SupplierID = 1 };
var produkt3 = new Product { ProductName = "Klocki Lego", UnitsOnStock = 20, SupplierID = 1 };

dbContext.Products.AddRange(produkt1, produkt2, produkt3);
dbContext.SaveChanges();

var faktura1 = new Invoice
{
    InvoiceNumber = "F2025-001",
    InvoiceDate = new DateTime(2025, 5, 10)
};
var faktura2 = new Invoice
{
    InvoiceNumber = "F2025-002",
    InvoiceDate = new DateTime(2025, 5, 15)
};

dbContext.Invoices.AddRange(faktura1, faktura2);
dbContext.SaveChanges();

var sprzedaz1 = new InvoiceProduct
{
    InvoiceID = faktura1.InvoiceID,
    ProductID = produkt1.ProductID,
    Quantity = 2
};
var sprzedaz2 = new InvoiceProduct
{
    InvoiceID = faktura1.InvoiceID,
    ProductID = produkt2.ProductID,
    Quantity = 1
};
var sprzedaz3 = new InvoiceProduct
{
    InvoiceID = faktura2.InvoiceID,
    ProductID = produkt3.ProductID,
    Quantity = 3
};
var sprzedaz4 = new InvoiceProduct
{
    InvoiceID = faktura2.InvoiceID,
    ProductID = produkt1.ProductID,
    Quantity = 1
};

dbContext.InvoiceProducts.AddRange(sprzedaz1, sprzedaz2, sprzedaz3, sprzedaz4);
dbContext.SaveChanges();


var searched = "Rower";
var wybranyProdukt = dbContext.Products
    .SingleOrDefault(p => p.ProductName == searched);

if (wybranyProdukt == null)
{
    Console.WriteLine($"Produkt o nazwie {searched} nie istnieje.");
    return;
}

var fakturyZProduktami = dbContext.InvoiceProducts
    .Where(ip => ip.ProductID == wybranyProdukt.ProductID)
    .Include(ip => ip.Invoice)
    .ToList();

Console.WriteLine($"Faktury, w których sprzedano produkt '{searched}':");
foreach (var ip in fakturyZProduktami)
{
    Console.WriteLine($"- Numer faktury: {ip.Invoice.InvoiceNumber}, Data: {ip.Invoice.InvoiceDate:d}, Ilość: {ip.Quantity}");
}
