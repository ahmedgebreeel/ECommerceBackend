using ECommerce.Business.DTOs.Brands;

namespace ECommerce.Business.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllAsync();
        Task<BrandDto> GetByIdAsync(int id);
        Task<BrandDto> CreateAsync(CreateBrandDto dto);
        Task<BrandDto> UpdateAsync(int id, UpdateBrandDto dto);
        Task DeleteAsync(int id);
    }
}
