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
        public DbSet<Producto> Productos { get; set; } 
        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoProducto> CarritoProductos { get; set; }

        public DbSet<Valoracion> Valoraciones { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<MetodoPago>().ToTable("metodos_pago");
            modelBuilder.Entity<PedidoDetalle>().ToTable("pedido_detalles");
            modelBuilder.Entity<Valoracion>().ToTable("valoraciones");


        }
    }
}
