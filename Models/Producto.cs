using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDeliveryApi.Models
{
    [Table("productos")] // nombre exacto de la tabla
    public class Producto
    {
        [Key]
        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("imagen_url")]
        public string ImagenUrl { get; set; }

        [Column("stock")]
        public int Stock { get; set; }
    }
}
