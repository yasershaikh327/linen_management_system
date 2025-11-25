using CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD
{
    public class AppDbContext : DbContext
    {
         public AppDbContext(DbContextOptions<AppDbContext> options): base(options){ }
         public DbSet<Student> Students { get; set; }
    }
}
