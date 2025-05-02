using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppDeliveryApi.Data;
using AppDeliveryApi.Models;
using AppDeliveryApi.DTOs;
using System.Net.Http;
using System.Collections.Generic;

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

        // POST: /api/auth/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDTO dto)
        {
            if (!await VerificarCaptchaAsync(dto.CaptchaToken))
                return BadRequest(new { mensaje = "Captcha inválido." });

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
                EsAdmin = false
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        // POST: /api/auth/login
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

        // Generación del token JWT con usuarioId explícito
        private string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim("usuarioId", usuario.UsuarioId.ToString()),
                new Claim("id", usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.EsAdmin ? "admin" : "usuario")
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

        // Verifica reCAPTCHA con Google
        private async Task<bool> VerificarCaptchaAsync(string token)
        {
            var secret = _configuration["GoogleCaptcha:SecretKey"];
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", token)
            });

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var json = await response.Content.ReadAsStringAsync();

            return json.Contains("\"success\": true");
        }
    }
}
