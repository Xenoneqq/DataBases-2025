using Microsoft.EntityFrameworkCore;

public class Product {
    public int ProductID {get; set;}
    public required String ProductName { get; set; }
    public int UnitsOnStock {get; set;}

    public int SupplierID { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
}

public class Supplier
{
    public int SupplierID { get; set; }
    public required String CompanyName { get; set; }
    public String? Street { get; set; }
    public String? City { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
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
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceProduct> InvoiceProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Datasource=MyProductDatabase");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // relacja Supplier - Product
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierID);

            // relacja wiele‐do‐wielu przez encję InvoiceProduct
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



