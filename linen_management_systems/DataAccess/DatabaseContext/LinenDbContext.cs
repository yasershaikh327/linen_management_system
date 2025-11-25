using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LinenManagementAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Linen> Linens { get; set; }
    public DbSet<CartLog> CartLogs { get; set; }
    public DbSet<CartLogDetail> CartLogDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>().ToTable("Carts");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Location>().ToTable("Locations");
        modelBuilder.Entity<Linen>().ToTable("Linen");
        modelBuilder.Entity<CartLog>().ToTable("CartLog");
        modelBuilder.Entity<CartLogDetail>().ToTable("CartLogDetail");

    }
}