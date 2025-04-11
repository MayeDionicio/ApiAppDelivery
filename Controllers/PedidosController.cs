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
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize] // Puede ser admin o usuario
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] PedidoDTO dto)
        {
            var nuevoPedido = new Pedido
            {
                UsuarioId = dto.UsuarioId,
                Total = dto.Total,
                Estado = dto.Estado,
                FechaPedido = dto.FechaPedido,
                MetodoPagoId = dto.MetodoPagoId
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            foreach (var detalle in dto.Detalles)
            {
                var nuevoDetalle = new PedidoDetalle
                {
                    PedidoId = nuevoPedido.PedidoId,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario
                };

                _context.PedidoDetalles.Add(nuevoDetalle);
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pedido registrado con éxito", pedidoId = nuevoPedido.PedidoId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.MetodoPago)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(pedidos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
                return NotFound(new { mensaje = "Pedido no encontrado" });

            return Ok(pedido);
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
    }
}
