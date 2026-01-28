using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductgRPC.Data;
using ProductgRPC.Models;

namespace ProductgRPC.Services;

public class ProductosServiceImpl : ProductosService.ProductosServiceBase
{
    private readonly AppDbContext _db;
    public ProductosServiceImpl(AppDbContext db) => _db = db;

    public override async Task<ListResponse> Listar(Empty request, ServerCallContext context)
    {
        var items = await _db.Productos.AsNoTracking().ToListAsync();
        var resp = new ListResponse();
        resp.Items.AddRange(items.Select(ToDto));
        return resp;
    }

    public override async Task<ProductoDto> Obtener(IdRequest request, ServerCallContext context)
    {
        var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);
        if (p is null) throw new RpcException(new Status(StatusCode.NotFound, "Producto no existe"));
        return ToDto(p);
    }

    public override async Task<ProductoDto> Crear(CrearRequest request, ServerCallContext context)
    {
        if (request.Precio < 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Precio no puede ser negativo"));

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Nombre es obligatorio"));

        var p = new Producto
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion ?? "",
            Precio = (decimal)request.Precio
        };

        _db.Productos.Add(p);
        await _db.SaveChangesAsync();

        return ToDto(p);
    }

    public override async Task<ProductoDto> Actualizar(ActualizarRequest request, ServerCallContext context)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (p is null) throw new RpcException(new Status(StatusCode.NotFound, "Producto no existe"));

        if (request.Precio < 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Precio no puede ser negativo"));

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Nombre es obligatorio"));

        p.Nombre = request.Nombre;
        p.Descripcion = request.Descripcion ?? "";
        p.Precio = (decimal)request.Precio;

        await _db.SaveChangesAsync();
        return ToDto(p);
    }

    public override async Task<Empty> Eliminar(IdRequest request, ServerCallContext context)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (p is null) throw new RpcException(new Status(StatusCode.NotFound, "Producto no existe"));

        _db.Productos.Remove(p);
        await _db.SaveChangesAsync();

        return new Empty();
    }

    private static ProductoDto ToDto(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = (double)p.Precio
    };
}
