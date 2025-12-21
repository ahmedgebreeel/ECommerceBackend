using ECommerce.Business.DTOs.Brands.Admin;
using ECommerce.Business.DTOs.Brands.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Brands;

namespace ECommerce.Business.Interfaces
{
    public interface IBrandService
    {
        Task<PagedResponseDto<AdminBrandDto>> GetAllBrandsAdminAsync(AdminBrandSpecParams specParams);
        Task<AdminBrandDetailsDto> GetBrandDetailsAdminAsync(int brandId);
        Task<AdminBrandDetailsDto> CreateBrandAdminAsync(AdminCreateBrandDto dto);
        Task<AdminBrandDetailsDto> UpdateBrandAdminAsync(int brandId, AdminUpdateBrandDto dto);
        Task DeleteBrandAdminAsync(int brandId);
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
    }
}
