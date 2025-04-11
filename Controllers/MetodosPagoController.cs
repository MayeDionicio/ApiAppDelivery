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
    public class MetodosPagoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MetodosPagoController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Crear método de pago
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] MetodoPagoDTO dto)
        {
            var metodo = new MetodoPago
            {
                UsuarioId = dto.UsuarioId,
                Tipo = dto.Tipo,
                Detalles = dto.Detalles,
                Activo = dto.Activo
            };

            _context.MetodosPago.Add(metodo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Método de pago registrado correctamente." });
        }

        // 📋 Listar todos los métodos de pago
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Listar()
        {
            var metodos = await _context.MetodosPago.ToListAsync();
            return Ok(metodos);
        }

        // 🔄 Editar método de pago
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Editar(int id, [FromBody] MetodoPagoDTO dto)
        {
            var metodo = await _context.MetodosPago.FindAsync(id);
            if (metodo == null) return NotFound(new { mensaje = "Método de pago no encontrado." });

            metodo.Tipo = dto.Tipo;
            metodo.Detalles = dto.Detalles;
            metodo.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Método de pago actualizado correctamente." });
        }

        // ❌ Eliminar método de pago
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Eliminar(int id)
        {
            var metodo = await _context.MetodosPago.FindAsync(id);
            if (metodo == null) return NotFound(new { mensaje = "Método de pago no encontrado." });

            _context.MetodosPago.Remove(metodo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Método de pago eliminado correctamente." });
        }
    }
}
