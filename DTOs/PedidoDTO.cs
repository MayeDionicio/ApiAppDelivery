namespace AppDeliveryApi.DTOs
{
    public class PedidoDTO
    {
        public int UsuarioId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }
        public int MetodoPagoId { get; set; }
        public double StoreLat { get; set; }
        public double StoreLng { get; set; }
        public double CustomerLat { get; set; }
        public double CustomerLng { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();
    }

    public class PedidoDetalleDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Para mostrar nombre del producto en el response (opcional)
        public string? NombreProducto { get; set; }
    }
}
