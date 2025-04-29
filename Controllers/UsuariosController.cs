using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDeliveryApi.Data;
using AppDeliveryApi.Models;
using AppDeliveryApi.DTOs;

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
                    u.Telefono,
                    u.EsAdmin
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] EditarUsuarioDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;
            usuario.Direccion = dto.Direccion;
            usuario.Telefono = dto.Telefono;
            usuario.EsAdmin = dto.EsAdmin;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
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
