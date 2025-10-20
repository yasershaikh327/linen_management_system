// DataAccess/Datain/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ResturantApp.Models;

namespace ResturantApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> student { get; set; }  // example table
    }

}
