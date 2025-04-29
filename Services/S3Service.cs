using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AppDeliveryApi.Services
{
    public class S3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            _bucketName = configuration["AwsS3:BucketName"];

            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? configuration["AwsS3:AccessKey"];
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? configuration["AwsS3:SecretKey"];
            var region = configuration["AwsS3:Region"];

            _s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task<string> SubirImagenAsync(IFormFile archivo)
        {
            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);

            using var stream = archivo.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = nombreArchivo,
                InputStream = stream,
                ContentType = archivo.ContentType
            };

            await _s3Client.PutObjectAsync(request);

            return $"https://{_bucketName}.s3.amazonaws.com/{nombreArchivo}";
        }

        public async Task EliminarImagenAsync(string nombreArchivo)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = nombreArchivo
            };

            await _s3Client.DeleteObjectAsync(request);
        }
    }
}
