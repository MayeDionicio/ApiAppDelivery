using AppDeliveryApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("pedidos")]
public class Pedido
{
    [Column("pedido_id")]
    public int PedidoId { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("estado")]
    public string Estado { get; set; }

    [Column("fecha_pedido")]
    public DateTime FechaPedido { get; set; }

    [Column("metodo_pago_id")]
    public int MetodoPagoId { get; set; }

    // ✅ Nuevas columnas de coordenadas
    [Column("store_lat")]
    public double? StoreLat { get; set; }

    [Column("store_lng")]
    public double? StoreLng { get; set; }

    [Column("customer_lat")]
    public double? CustomerLat { get; set; }

    [Column("customer_lng")]
    public double? CustomerLng { get; set; }

    public Usuario Usuario { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public List<PedidoDetalle> Detalles { get; set; }
}
