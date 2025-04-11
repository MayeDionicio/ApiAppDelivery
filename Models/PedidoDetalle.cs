namespace AppDeliveryApi.Models
{
    public class PedidoDetalle
    {
        public int PedidoDetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relaciones
        public Pedido Pedido { get; set; }
        public Producto Producto { get; set; }
    }
}
