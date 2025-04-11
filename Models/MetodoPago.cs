namespace AppDeliveryApi.Models
{
    public class MetodoPago
    {
        public int MetodoPagoId { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Detalles { get; set; } // Podés usar un tipo más complejo si es JSON
        public bool Activo { get; set; }

        // Relación inversa (opcional)
        public List<Pedido> Pedidos { get; set; }
    }
}
