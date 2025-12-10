using ECommerce.Business.DTOs.ProductImages;

namespace ECommerce.Business.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetAllAsync(int productId);
        Task<ProductImageDto> AddImageAsync(int productId, AddProductImageDto dto);
        Task SetMainImageAsync(int productIdFromRotue, int imageId);
        Task DeleteImageAsync(int productIdFromRoute, int imageId);
    }
}
