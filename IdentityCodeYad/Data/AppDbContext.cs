using IdentityCodeYad.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityCodeYad.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}