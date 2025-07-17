using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Postkit.Posts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Posts.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly ILogger<CloudinaryService> logger;
        private readonly Cloudinary cloudinary;

        public CloudinaryService(IConfiguration config, ILogger<CloudinaryService> logger)
        {
            this.logger = logger; 
            var settings = config.GetSection("Cloudinary");
            var account = new Account(
                settings["CloudName"],
                settings["ApiKey"],
                settings["ApiSecret"]
            );
            
            if (string.IsNullOrEmpty(account.Cloud) || string.IsNullOrEmpty(account.ApiKey) || string.IsNullOrEmpty(account.ApiSecret))
            {
                logger.LogError("Cloudinary configuration is missing or incomplete.");
                throw new InvalidOperationException("Cloudinary configuration is invalid.");
            }

            cloudinary = new Cloudinary(new Account(account.Cloud, account.ApiKey, account.ApiSecret));

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
