using Microsoft.EntityFrameworkCore;
using AppDeliveryApi.Models;

namespace AppDeliveryApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; } // 👈 Esto está bien, aunque en la BD sea "productos"
        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoProducto> CarritoProductos { get; set; }

        public DbSet<Valoracion> Valoraciones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 Aquí asegurás que EF Core use el nombre exacto "productos" (en minúscula)
            modelBuilder.Entity<Producto>().ToTable("productos");

            // También podrías mapear otras tablas si las tenés en minúsculas:
            // modelBuilder.Entity<Usuario>().ToTable("usuarios");
        }
    }
}
