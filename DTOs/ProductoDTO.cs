using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AppDeliveryApi.DTOs
{
    public class ProductoDTO
    {
        [FromForm]
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string Nombre { get; set; }

        [FromForm]
        public string? Descripcion { get; set; }

        [FromForm]
        public decimal Precio { get; set; }

        [FromForm]
        public int Stock { get; set; }

        [FromForm]
        public IFormFile? Imagen { get; set; }
    }
}
