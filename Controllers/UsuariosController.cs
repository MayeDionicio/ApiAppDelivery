using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDeliveryApi.Data;
using AppDeliveryApi.Models;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        
        [Authorize(Roles = "admin")]
        [HttpGet("listar")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new {
                    u.UsuarioId,
                    u.Nombre,
                    u.Email,
                    u.EsAdmin
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        
        [Authorize(Roles = "admin")]
        [HttpPut("asignar-admin/{id}")]
        public async Task<IActionResult> AsignarAdmin(int id, [FromQuery] bool esAdmin)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            usuario.EsAdmin = esAdmin;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Rol actualizado. Usuario ahora esAdmin: {usuario.EsAdmin}" });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario eliminado correctamente." });
        }

    }
}
