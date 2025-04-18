﻿namespace AppDeliveryApi.DTOs
{
    public class PedidoDTO
    {
        public int UsuarioId { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }
        public int MetodoPagoId { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();
    }

    public class PedidoDetalleDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
