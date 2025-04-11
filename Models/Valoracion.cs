namespace AppDeliveryApi.Models
{
    public class Valoracion
    {
        public int ValoracionId { get; set; }
        public int ProductoId { get; set; }
        public int UsuarioId { get; set; }
        public int Calificacion { get; set; } // Por ejemplo: 1 a 5 estrellas
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }

        // Relaciones
        public Producto Producto { get; set; }
        public Usuario Usuario { get; set; }
    }
}
