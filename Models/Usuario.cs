using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDeliveryApi.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("contrasena_hash")]
        public string ContrasenaHash { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("es_admin")]
        public bool EsAdmin { get; set; }

        public string? FotoUrl { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
