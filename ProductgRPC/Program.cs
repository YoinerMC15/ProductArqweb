using Microsoft.EntityFrameworkCore;
using ProductgRPC.Data;
using ProductgRPC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=productos_grpc.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGrpcService<ProductosServiceImpl>();
app.MapGet("/", () => "Servidor gRPC activo.");
app.Run();
