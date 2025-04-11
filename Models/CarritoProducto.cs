using AppDeliveryApi.Models;

public class CarritoProducto
{
    public int CarritoProductoId { get; set; }
    public int CarritoId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }

    public Producto Producto { get; set; }
}
