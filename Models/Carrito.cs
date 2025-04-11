public class Carrito
{
    public int CarritoId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }

    public List<CarritoProducto> Productos { get; set; }
}
