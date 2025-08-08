using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Postkit.Shared.Interfaces.Cloudinary;

namespace Postkit.Infrastructure.Media
{
    public class CloudinaryService : ICloudinaryUploader
    {
        private readonly ILogger<CloudinaryService> logger;
        private readonly Cloudinary cloudinary;
        private readonly CloudinarySettings settings;

        public CloudinaryService(IOptions<CloudinarySettings> options, ILogger<CloudinaryService> logger)
        {
            this.logger = logger;
            this.settings = options.Value;

            if (string.IsNullOrWhiteSpace(settings.CloudName) ||
                string.IsNullOrWhiteSpace(settings.ApiKey) ||
                string.IsNullOrWhiteSpace(settings.ApiSecret))
            {
                logger.LogError("Cloudinary configuration is missing or incomplete.");
                throw new InvalidOperationException("Cloudinary configuration is invalid.");
            }

            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            cloudinary = new Cloudinary(account);

        }
        public async Task<string> UploadMediaAsync(IFormFile file)
        {
            var folder = "postkit";
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folder
            };

            string contentType = file.ContentType.ToLower();

            if (contentType.StartsWith("image"))
            {
                var imageUploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folder
                };

                var uploadResult = await cloudinary.UploadAsync(imageUploadParams);
                return uploadResult.SecureUrl?.AbsoluteUri ?? throw new Exception("Image upload failed.");
            }
            else if (contentType.StartsWith("video"))
            {
                var videoUploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folder
                };

                var uploadResult = await cloudinary.UploadAsync(videoUploadParams);
                return uploadResult.SecureUrl?.AbsoluteUri ?? throw new Exception("Video upload failed.");
            }

            throw new NotSupportedException($"Unsupported media type: {file.ContentType}");
        }

    }
}
