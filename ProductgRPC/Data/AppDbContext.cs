using Microsoft.EntityFrameworkCore;
using ProductgRPC.Models;

namespace ProductgRPC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}
