using Microsoft.EntityFrameworkCore;

public class Product {
    public int ProductID {get; set;}
    public required String ProductName { get; set; }
    public int UnitsOnStock {get; set;}

    public int SupplierID { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}

public abstract class Company
{
    public int CompanyID { get; set; }            
    public required string CompanyName { get; set; }    
    public string? Street { get; set; }              
    public string? City { get; set; }                   
    public string? ZipCode { get; set; }       
}

public class Supplier : Company
{
    public string? BankAccountNumber { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Customer : Company
{
    public decimal Discount { get; set; }
}

public class Invoice
{
    public int InvoiceID { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}

public class InvoiceProduct
{
    public int InvoiceID { get; set; }
    public Invoice Invoice { get; set; } = default!;
    public int ProductID { get; set; }
    public Product Product { get; set; } = default!;
    public int Quantity { get; set; }
}


public class DatabaseContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceProduct> InvoiceProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Datasource=MyProductDatabase");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .HasDiscriminator<string>("CompanyType")
            .HasValue<Supplier>("Supplier")
            .HasValue<Customer>("Customer");

        modelBuilder.Entity<Supplier>()
            .HasMany(s => s.Products)
            .WithOne(p => p.Supplier)
            .HasForeignKey(p => p.SupplierID);

        modelBuilder.Entity<InvoiceProduct>()
            .HasKey(ip => new { ip.InvoiceID, ip.ProductID });

        modelBuilder.Entity<InvoiceProduct>()
            .HasOne(ip => ip.Invoice)
            .WithMany(inv => inv.InvoiceProducts)
            .HasForeignKey(ip => ip.InvoiceID);

        modelBuilder.Entity<InvoiceProduct>()
            .HasOne(ip => ip.Product)
            .WithMany(prod => prod.InvoiceProducts)
            .HasForeignKey(ip => ip.ProductID);
    }
}



