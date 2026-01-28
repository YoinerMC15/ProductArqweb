using Microsoft.EntityFrameworkCore;
using ProductGraphQL.Data;
using ProductGraphQL.Models;

namespace ProductGraphQL.GraphQL;

public class Query
{
    public Task<List<Producto>> Productos([Service] AppDbContext db) =>
        db.Productos.AsNoTracking().ToListAsync();

    public Task<Producto?> ProductoPorId(int id, [Service] AppDbContext db) =>
        db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
