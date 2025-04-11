using AppDeliveryApi.Data;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // POST: api/valoraciones
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] ValoracionDTO dto)
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

        // GET: api/valoraciones/producto/5
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> ObtenerPorProducto(int productoId)
        {
            var valoraciones = await _context.Valoraciones
                .Where(v => v.ProductoId == productoId)
                .ToListAsync();

            return Ok(valoraciones);
        }

        // DELETE: api/valoraciones/5
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
