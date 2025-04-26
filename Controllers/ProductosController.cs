using AppDeliveryApi.Data;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.Models;
using AppDeliveryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly S3Service _s3Service;

        public ProductosController(AppDbContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var productos = await _context.Productos.ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoDTO dto)
        {
            var nuevo = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                ImagenUrl = dto.ImagenUrl,
                Stock = dto.Stock
            };

            _context.Productos.Add(nuevo);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producto creado." });
        }

        

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] ProductoDTO dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Precio = dto.Precio;
            producto.ImagenUrl = dto.ImagenUrl;
            producto.Stock = dto.Stock;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producto actualizado." });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producto eliminado." });
        }
    }
}
