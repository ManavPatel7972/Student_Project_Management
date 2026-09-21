using Microsoft.AspNetCore.Http;

namespace Backend.Services
{
    public interface IProfileImageService
    {
        Task<ProfileImageUploadResult> UploadAsync(IFormFile file, int userId);
        Task DeleteAsync(string imageUrl);
    }


    public class ProfileImageUploadResult
    {
        public string ImageUrl { get; }
        public string PublicId { get; }

        public ProfileImageUploadResult(string imageUrl, string publicId)
        {
            ImageUrl = imageUrl;
            PublicId = publicId;
        }
    }
}
