namespace AppDeliveryApi.DTOs
{
    public class MetodoPagoDTO
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Detalles { get; set; }
        public bool Activo { get; set; }
    }
}
