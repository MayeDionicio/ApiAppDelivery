namespace AppDeliveryApi.DTOs
{
    public class ProductoDTO
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int Stock { get; set; }
    }
}
