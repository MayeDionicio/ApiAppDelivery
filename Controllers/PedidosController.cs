using AppDeliveryApi.Data;
using AppDeliveryApi.DTOs;
using AppDeliveryApi.Models;
using AppDeliveryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace AppDeliveryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly EmailService _emailService;

        private const string ClaveQr = "12345678901234567890123456789012"; // 32 bytes

        public PedidosController(AppDbContext context, IWebHostEnvironment env, EmailService emailService)
        {
            _context = context;
            _env = env;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] PedidoDTO dto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
                if (usuario == null)
                    return BadRequest(new { mensaje = "Usuario no encontrado" });

                var nuevoPedido = new Pedido
                {
                    UsuarioId = dto.UsuarioId,
                    Total = dto.Total,
                    Estado = dto.Estado,
                    FechaPedido = DateTime.SpecifyKind(dto.FechaPedido, DateTimeKind.Utc),
                    MetodoPagoId = dto.MetodoPagoId
                };

                _context.Pedidos.Add(nuevoPedido);
                await _context.SaveChangesAsync();

                foreach (var detalle in dto.Detalles)
                {
                    _context.PedidoDetalles.Add(new PedidoDetalle
                    {
                        PedidoId = nuevoPedido.PedidoId,
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario
                    });
                }

                await _context.SaveChangesAsync();

                var url = $"https://deliverylp.shop/api/Pedidos/entregar/{nuevoPedido.PedidoId}";
                var qrContenido = EncriptarQr(url);
                var qrBytes = GenerarQr(qrContenido);
                var fileName = $"pedido_{nuevoPedido.PedidoId}.png";
                var savePath = Path.Combine(_env.WebRootPath, "qr", fileName);
                System.IO.File.WriteAllBytes(savePath, qrBytes);

                await _emailService.EnviarCorreoConQrYDetalle(
                    usuario,
                    nuevoPedido,
                    await _context.PedidoDetalles.Include(p => p.Producto)
                        .Where(p => p.PedidoId == nuevoPedido.PedidoId).ToListAsync(),
                    savePath
                );

                return Ok(new
                {
                    mensaje = "Pedido registrado con éxito",
                    pedidoId = nuevoPedido.PedidoId,
                    qr = $"https://deliverylp.shop/qr/{fileName}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.MetodoPago)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .ToListAsync();

            var lista = pedidos.Select(p => new PedidoResponseDTO
            {
                PedidoId = p.PedidoId,
                Total = p.Total,
                Estado = p.Estado,
                FechaPedido = p.FechaPedido,
                Usuario = new UsuarioSimpleDTO
                {
                    UsuarioId = p.Usuario.UsuarioId,
                    Nombre = p.Usuario.Nombre,
                    Email = p.Usuario.Email
                },
                MetodoPago = new MetodoPagoSimpleDTO
                {
                    MetodoPagoId = p.MetodoPago.MetodoPagoId,
                    Tipo = p.MetodoPago.Tipo
                },
                Detalles = p.Detalles.Select(d => new PedidoDetalleDTO
                {
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            });

            return Ok(lista);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var p = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.MetodoPago)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (p == null)
                return NotFound(new { mensaje = "Pedido no encontrado" });

            var dto = new PedidoResponseDTO
            {
                PedidoId = p.PedidoId,
                Total = p.Total,
                Estado = p.Estado,
                FechaPedido = p.FechaPedido,
                Usuario = new UsuarioSimpleDTO
                {
                    UsuarioId = p.Usuario.UsuarioId,
                    Nombre = p.Usuario.Nombre,
                    Email = p.Usuario.Email
                },
                MetodoPago = new MetodoPagoSimpleDTO
                {
                    MetodoPagoId = p.MetodoPago.MetodoPagoId,
                    Tipo = p.MetodoPago.Tipo
                },
                Detalles = p.Detalles.Select(d => new PedidoDetalleDTO
                {
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            return Ok(dto);
        }

        [Authorize]
        [HttpPost("entregar-qr")]
        public async Task<IActionResult> EntregarDesdeQr([FromBody] string contenidoQr)
        {
            try
            {
                var desencriptado = DesencriptarQr(contenidoQr);
                if (!desencriptado.Contains("/entregar/"))
                    return BadRequest(new { mensaje = "QR inválido" });

                var id = int.Parse(desencriptado.Split("/").Last());

                return await EntregarConQr(id);
            }
            catch
            {
                return BadRequest(new { mensaje = "Error al desencriptar el QR" });
            }
        }

        [Authorize]
        [HttpGet("entregar/{id}")]
        public async Task<IActionResult> EntregarConQr(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null || pedido.Estado == "Entregado")
                return BadRequest(new { mensaje = "Pedido inválido o ya entregado" });

            foreach (var d in pedido.Detalles)
            {
                if (d.Producto.Stock < d.Cantidad)
                    return BadRequest(new { mensaje = $"Stock insuficiente para el producto ID {d.ProductoId}" });

                d.Producto.Stock -= d.Cantidad;
            }

            pedido.Estado = "Entregado";
            await _context.SaveChangesAsync();

            await _emailService.EnviarCorreoEntregaHtml(pedido.Usuario, pedido);


            return Ok(new { mensaje = "Pedido entregado correctamente vía QR" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/entregar")]
        public async Task<IActionResult> EntregarManual(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null || pedido.Estado == "Entregado")
                return BadRequest(new { mensaje = "Pedido inválido o ya entregado" });

            foreach (var d in pedido.Detalles)
            {
                if (d.Producto.Stock < d.Cantidad)
                    return BadRequest(new { mensaje = $"Stock insuficiente para el producto ID {d.ProductoId}" });

                d.Producto.Stock -= d.Cantidad;
            }

            pedido.Estado = "Entregado";
            await _context.SaveChangesAsync();

            await _emailService.EnviarCorreoBasico(pedido.Usuario.Email,
                "📦 Pedido entregado",
                $"Hola {pedido.Usuario.Nombre}, tu pedido #{pedido.PedidoId} fue entregado correctamente.");

            return Ok(new { mensaje = "Pedido entregado correctamente manualmente" });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound(new { mensaje = "Pedido no encontrado" });

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pedido eliminado correctamente" });
        }

        private byte[] GenerarQr(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(10); // tamaño más pequeño
        }

        private string EncriptarQr(string texto)
        {
            if (ClaveQr.Length != 32)
                throw new Exception("La clave QR debe tener exactamente 32 bytes");

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ClaveQr);
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            var plainBytes = Encoding.UTF8.GetBytes(texto);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(iv.Concat(encryptedBytes).ToArray());
        }

        private string DesencriptarQr(string texto)
        {
            var fullCipher = Convert.FromBase64String(texto);
            var iv = fullCipher.Take(16).ToArray();
            var cipher = fullCipher.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ClaveQr);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
