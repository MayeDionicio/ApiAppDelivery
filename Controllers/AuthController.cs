using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppDeliveryApi.Data;
using AppDeliveryApi.Models;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.DTOs;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Registro para usuarios normales (no admins)
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo ya está registrado." });

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                ContrasenaHash = hash,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                EsAdmin = false // 👈 siempre será usuario normal desde aquí
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        // ✅ Login y generación de token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });

            bool esValido = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);
            if (!esValido)
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });

            var token = GenerarToken(usuario);

            return Ok(new
            {
                mensaje = "Login exitoso",
                token,
                rol = usuario.EsAdmin ? "admin" : "usuario"
            });
        }

        // 🔐 Generar JWT con rol como claim
        private string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.EsAdmin ? "admin" : "usuario") // 👈 rol aquí
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
