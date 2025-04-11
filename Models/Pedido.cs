using AppDeliveryApi.Models;

public class Pedido
{
    public int PedidoId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; }
    public DateTime FechaPedido { get; set; }
    public int MetodoPagoId { get; set; }

    // ✅ Propiedades de navegación
    public Usuario Usuario { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public List<PedidoDetalle> Detalles { get; set; }
}
