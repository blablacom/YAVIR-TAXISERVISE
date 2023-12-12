using Microsoft.EntityFrameworkCore;
using SearchForDriversWebApp.Models;

/// <summary>
///         
/// </summary>
namespace SearchForDriversWebApp
{
    public class DriverDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
        {
           //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
