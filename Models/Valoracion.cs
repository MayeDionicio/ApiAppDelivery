using AppDeliveryApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Valoracion
{
    [Column("valoracion_id")]
    public int ValoracionId { get; set; }

    [Column("producto_id")]
    public int ProductoId { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("puntuacion")]
    public int Calificacion { get; set; }

    [Column("comentario")]
    public string Comentario { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; }

    public Producto Producto { get; set; }
    public Usuario Usuario { get; set; }
}
