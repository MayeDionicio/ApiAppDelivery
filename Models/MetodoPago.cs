using System.ComponentModel.DataAnnotations.Schema;

namespace AppDeliveryApi.Models
{
    [Table("metodos_pago")]
    public class MetodoPago
    {
        [Column("metodo_pago_id")]
        public int MetodoPagoId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("detalles")]
        public string Detalles { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        // Relación inversa (opcional)
        public List<Pedido> Pedidos { get; set; }
    }
}
