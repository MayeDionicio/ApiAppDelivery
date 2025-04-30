using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using AppDeliveryApi.Models;
using System.Net.Mail;

namespace AppDeliveryApi.Services
{
    public class EmailService
    {
        public EmailService()
        {
            // No se necesita IConfiguration
        }

        public async Task EnviarCorreoConQrYDetalle(Usuario usuario, Pedido pedido, List<PedidoDetalle> detalles, string rutaQr)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DeliveryLP", Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(usuario.Email));
            message.Subject = $"📦 Pedido #{pedido.PedidoId} confirmado";

            var builder = new BodyBuilder();

            string tablaHtml = "";
            foreach (var detalle in detalles)
            {
                tablaHtml += $"<tr><td>{detalle.Producto.Nombre}</td><td>{detalle.Cantidad}</td><td>Q {detalle.PrecioUnitario:0.00}</td></tr>";
            }

            var image = builder.LinkedResources.Add(rutaQr);
            image.ContentId = MimeUtils.GenerateMessageId();

            string cuerpoHtml = File.ReadAllText("Templates/PlantillaCorreo.html")
                .Replace("{{NOMBRE_CLIENTE}}", usuario.Nombre)
                .Replace("{{ID_PEDIDO}}", pedido.PedidoId.ToString())
                .Replace("{{TABLA_PRODUCTOS}}", tablaHtml)
                .Replace("{{TOTAL_PEDIDO}}", pedido.Total.ToString("0.00"))
                .Replace("cid:qr", $"cid:{image.ContentId}");

            builder.HtmlBody = cuerpoHtml;
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("EMAIL_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587"),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USER"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            );
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task EnviarCorreoBasico(string destinatario, string asunto, string cuerpo)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DeliveryLP", Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(destinatario));
            message.Subject = asunto;

            var builder = new BodyBuilder
            {
                HtmlBody = $"<p>{cuerpo}</p>"
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("EMAIL_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587"),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USER"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            );
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task EnviarCorreoEntregaHtml(Usuario usuario, Pedido pedido)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DeliveryLP", Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(usuario.Email));
            message.Subject = $"📦 Pedido #{pedido.PedidoId} entregado";

            var builder = new BodyBuilder();

            string cuerpoHtml = File.ReadAllText("Templates/EntregaConfirmada.html")
                .Replace("{{NOMBRE_CLIENTE}}", usuario.Nombre)
                .Replace("{{ID_PEDIDO}}", pedido.PedidoId.ToString());

            builder.HtmlBody = cuerpoHtml;
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("EMAIL_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587"),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_USER"),
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
            );
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
