using Backend.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Backend.Services
{
    public class CloudinaryProfileImageService : IProfileImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryProfileImageService(IOptions<CloudinarySettings> settings)
        {
            var value = settings.Value;

            Console.WriteLine("Cloudinary DATA =======>" + value);
            // System.Console.WriteLine("-----------------------------------------");
            // Console.WriteLine("CloudName ==========================>" + value.CloudName);
            // System.Console.WriteLine("APikey===========>" + value.ApiKey );
            // System.Console.WriteLine("ApiSecret ===========>" + value.ApiSecret );

            if (string.IsNullOrWhiteSpace(value.CloudName) ||
                string.IsNullOrWhiteSpace(value.ApiKey) ||
                string.IsNullOrWhiteSpace(value.ApiSecret))
            {
                throw new InvalidOperationException("Cloudinary settings are missing.");
            }

            _cloudinary = new Cloudinary(new Account(value.CloudName, value.ApiKey, value.ApiSecret));
        }

        public async Task<ProfileImageUploadResult> UploadAsync(IFormFile file, int userId)
        {
            await using var stream = file.OpenReadStream();

            var upload = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "student-project-management/profiles",
                PublicId = $"user-{userId}",
                Overwrite = true,
                Invalidate = true,
                Transformation = new Transformation()
                    .Width(400)
                    .Height(400)
                    .Crop("fill")
                    .Gravity("face")
            };

            var result = await _cloudinary.UploadAsync(upload);

            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }

            System.Console.WriteLine(" Upload Result ====>>>>" + result);
            return new ProfileImageUploadResult(result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task DeleteAsync(string imageUrl)
        {
            var publicId = GetPublicId(imageUrl);
            if (string.IsNullOrWhiteSpace(publicId)) return;

            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                Invalidate = true,
                ResourceType = ResourceType.Image
            });

            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }
        }
        
        private static string? GetPublicId(string imageUrl)
        {
    
            const string marker = "student-project-management/profiles/user-";
            
            var index = imageUrl.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0) return null;

            var value = imageUrl.Substring(index);
            var extensionIndex = value.LastIndexOf('.');
            return extensionIndex > -1 ? value.Substring(0, extensionIndex) : value;
        }
    }
}
