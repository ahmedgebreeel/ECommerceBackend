using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products;
using ECommerce.Core.Specifications;

namespace ECommerce.Business.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponseDto<ProductDto>> GetAllAsync(ProductSpecParams specParams);
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);

    }
}
