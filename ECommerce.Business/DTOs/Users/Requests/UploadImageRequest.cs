using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.DTOs.Users.Requests
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
