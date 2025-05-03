namespace AppDeliveryApi.DTOs
{
    public class PedidoResponseDTO
    {
        public int PedidoId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }

    
        public double? StoreLat { get; set; }
        public double? StoreLng { get; set; }
        public double? CustomerLat { get; set; }
        public double? CustomerLng { get; set; }

        public UsuarioSimpleDTO Usuario { get; set; }
        public MetodoPagoSimpleDTO MetodoPago { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }
}
