using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Interfaces.Cloudinary
{
    public interface ICloudinaryUploader
    {
        Task<string> UploadMediaAsync(IFormFile file);
    }
}
