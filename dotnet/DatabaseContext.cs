using Microsoft.EntityFrameworkCore;

public class Product {
    public int ProductID {get; set;}
    public String ProductName { get; set; }
    public int UnitsOnStock {get; set;}

    public int? SupplierID { get; set; }
    public Supplier? Supplier { get; set; }
}

public class Supplier {
    public int SupplierID {get; set;}
    public String CompanyName {get; set;}
    public String? Street {get; set;} 
    public String? City {get; set;}

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class DatabaseContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Datasource=MyProductDatabase");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierID);
    }
}



