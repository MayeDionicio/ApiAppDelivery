namespace AppDeliveryApi.DTOs
{
    public class RegistroUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public bool EsAdmin { get; set; }
    }
}
