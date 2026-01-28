using Microsoft.EntityFrameworkCore;
using ProductGraphQL.Data;
using ProductGraphQL.Models;

namespace ProductGraphQL.GraphQL;

public record ProductoInput(string Nombre, string Descripcion, decimal Precio);

public class Mutation
{
    public async Task<Producto> CrearProducto(ProductoInput input, [Service] AppDbContext db)
    {
        if (input.Precio < 0) throw new GraphQLException("Precio no puede ser negativo.");
        var p = new Producto { Nombre = input.Nombre, Descripcion = input.Descripcion, Precio = input.Precio };
        db.Productos.Add(p);
        await db.SaveChangesAsync();
        return p;
    }

    public async Task<Producto> ActualizarProducto(int id, ProductoInput input, [Service] AppDbContext db)
    {
        var p = await db.Productos.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new GraphQLException("Producto no existe.");

        if (input.Precio < 0) throw new GraphQLException("Precio no puede ser negativo.");

        p.Nombre = input.Nombre; p.Descripcion = input.Descripcion; p.Precio = input.Precio;
        await db.SaveChangesAsync();
        return p;
    }

    public async Task<bool> EliminarProducto(int id, [Service] AppDbContext db)
    {
        var p = await db.Productos.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new GraphQLException("Producto no existe.");

        db.Productos.Remove(p);
        await db.SaveChangesAsync();
        return true;
    }
}
