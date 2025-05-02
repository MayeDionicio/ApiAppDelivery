using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AppDeliveryApi.Services
{
    public class S3Service
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            _configuration = configuration;

            // 🔐 Leer claves de entorno o fallback a appsettings
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? configuration["AwsS3:AccessKey"];
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? configuration["AwsS3:SecretKey"];
            var region = configuration["AwsS3:Region"];

            // ✅ Validaciones claras para debug
            if (string.IsNullOrEmpty(accessKey))
                throw new Exception("⚠️ AWS_ACCESS_KEY_ID no está configurado.");
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("⚠️ AWS_SECRET_ACCESS_KEY no está configurado.");
            if (string.IsNullOrEmpty(region))
                throw new Exception("⚠️ AwsS3:Region no está configurado en appsettings.json.");

            _s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task<string> SubirImagenAsync(IFormFile archivo, string tipo = "producto")
        {
            var bucket = tipo == "perfil"
                ? _configuration["AwsS3:BucketPerfil"]
                : _configuration["AwsS3:BucketName"];

            if (string.IsNullOrEmpty(bucket))
                throw new Exception($"⚠️ Bucket no configurado para tipo '{tipo}'.");

            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(archivo.FileName);

            using var stream = archivo.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = nombreArchivo,
                InputStream = stream,
                ContentType = archivo.ContentType
            };

            await _s3Client.PutObjectAsync(request);

            return $"https://{bucket}.s3.amazonaws.com/{nombreArchivo}";
        }

        public async Task EliminarImagenAsync(string urlImagen, string tipo = "producto")
        {
            var bucket = tipo == "perfil"
                ? _configuration["AwsS3:BucketPerfil"]
                : _configuration["AwsS3:BucketName"];

            if (string.IsNullOrEmpty(bucket))
                throw new Exception($"⚠️ Bucket no configurado para tipo '{tipo}'.");

            var uri = new Uri(urlImagen);
            var key = uri.AbsolutePath.TrimStart('/');

            var request = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }
    }
}
