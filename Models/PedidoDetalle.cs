using AppDeliveryApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("pedido_detalles")]
public class PedidoDetalle
{
    [Column("pedido_detalle_id")]
    public int PedidoDetalleId { get; set; }

    [Column("pedido_id")]
    public int PedidoId { get; set; }

    [Column("producto_id")]
    public int ProductoId { get; set; }

    [Column("cantidad")] 
    public int Cantidad { get; set; }

    [Column("precio_unitario")]
    public decimal PrecioUnitario { get; set; }

    public Pedido Pedido { get; set; }
    public Producto Producto { get; set; }
}
