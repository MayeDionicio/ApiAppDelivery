namespace AppDeliveryApi.DTOs
{
    public class PedidoResponseDTO
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }

        public UsuarioSimpleDTO Usuario { get; set; }
        public MetodoPagoSimpleDTO MetodoPago { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }
}
