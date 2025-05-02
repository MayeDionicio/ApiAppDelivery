using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppDeliveryApi.Data;
using AppDeliveryApi.Models;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.Services;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly S3Service _s3Service;

        public UsuariosController(AppDbContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        // ==== ADMIN ====

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
                    u.EsAdmin,
                    u.Direccion,
                    u.FotoUrl
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

        // ==== PERFIL DEL USUARIO ====

        [Authorize]
        [HttpPut("perfil/editar")]
        public async Task<IActionResult> EditarPerfil([FromBody] EditarPerfilDTO dto)
        {
            var claimId = User?.FindFirst("id")?.Value;
            if (claimId == null)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var userId = int.Parse(claimId);
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null) return NotFound();

            usuario.Nombre = dto.Nombre;
            usuario.Telefono = dto.Telefono;
            usuario.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Perfil actualizado correctamente." });
        }
        [Consumes("multipart/form-data")]
        [Authorize]
        [HttpPost("perfil/foto")]
        public async Task<IActionResult> CambiarFotoPerfil([FromForm] IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo no válido.");

            var claimId = User?.FindFirst("id")?.Value;
            if (claimId == null)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var userId = int.Parse(claimId);
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null) return NotFound();

            if (!string.IsNullOrEmpty(usuario.FotoUrl))
                await _s3Service.EliminarImagenAsync(usuario.FotoUrl, "perfil");

            var url = await _s3Service.SubirImagenAsync(archivo, "perfil");
            usuario.FotoUrl = url;

            await _context.SaveChangesAsync();
            return Ok(new { fotoUrl = url });
        }


        [Authorize]
        [HttpDelete("perfil/foto")]
        public async Task<IActionResult> EliminarFotoPerfil()
        {
            var claimId = User?.FindFirst("id")?.Value;
            if (claimId == null)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var userId = int.Parse(claimId);
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null || string.IsNullOrEmpty(usuario.FotoUrl))
                return NotFound(new { mensaje = "No hay foto para eliminar." });

            await _s3Service.EliminarImagenAsync(usuario.FotoUrl, "perfil");
            usuario.FotoUrl = null;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Foto eliminada correctamente." });
        }

        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var claimId = User?.FindFirst("id")?.Value;
            if (claimId == null)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var userId = int.Parse(claimId);
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            return Ok(new
            {
                usuario.UsuarioId,
                usuario.Nombre,
                usuario.Email,
                usuario.Telefono,
                usuario.Direccion,
                usuario.FotoUrl,
                Rol = usuario.EsAdmin ? "admin" : "usuario"
            });
        }

    }
}
