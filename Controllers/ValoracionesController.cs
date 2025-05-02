using AppDeliveryApi.Data;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValoracionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ValoracionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] ValoracionDTO dto)
        {
            try
            {
                var valoracion = new Valoracion
                {
                    ProductoId = dto.ProductoId,
                    UsuarioId = dto.UsuarioId,
                    Calificacion = dto.Calificacion,
                    Comentario = dto.Comentario,
                    Fecha = DateTime.UtcNow
                };

                _context.Valoraciones.Add(valoracion);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Valoración registrada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> ObtenerPorProducto(int productoId)
        {
            var valoraciones = await _context.Valoraciones
                .Include(v => v.Producto)
                .Include(v => v.Usuario)
                .Where(v => v.ProductoId == productoId)
                .ToListAsync();

            return Ok(valoraciones);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var valoracion = await _context.Valoraciones.FindAsync(id);
            if (valoracion == null)
                return NotFound();

            _context.Valoraciones.Remove(valoracion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Valoración eliminada." });
        }
    }
}
