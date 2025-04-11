using AppDeliveryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarritoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("agregar")]
    public async Task<IActionResult> AgregarProducto([FromBody] AgregarAlCarritoDTO dto)
    {
        var carrito = await _context.Carritos
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.UsuarioId == dto.UsuarioId);

        if (carrito == null)
        {
            carrito = new Carrito
            {
                UsuarioId = dto.UsuarioId,
                FechaCreacion = DateTime.UtcNow,
                Productos = new List<CarritoProducto>()
            };
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
        }

        var productoExistente = carrito.Productos.FirstOrDefault(p => p.ProductoId == dto.ProductoId);
        if (productoExistente != null)
        {
            productoExistente.Cantidad += dto.Cantidad;
        }
        else
        {
            carrito.Productos.Add(new CarritoProducto
            {
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Producto agregado al carrito" });
    }

    [HttpGet("{usuarioId}")]
    public async Task<IActionResult> ObtenerCarrito(int usuarioId)
    {
        var carrito = await _context.Carritos
            .Include(c => c.Productos)
            .ThenInclude(cp => cp.Producto)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null)
            return NotFound(new { mensaje = "Carrito no encontrado" });

        return Ok(carrito);
    }
}
