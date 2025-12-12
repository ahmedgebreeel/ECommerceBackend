using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger) : IProductService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductService> _logger = logger;

        public async Task<PagedResponseDto<ProductDto>> GetAllAsync(ProductSpecParams specParams)
        {
            //Create basic query
            var query = _context.Products.AsNoTracking().AsQueryable();
            var response = new PagedResponseDto<ProductDto>();
            //1.Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(specParams.Search));
                response.Search = specParams.Search;

            }

            //2.Filter Category / Brand / Price Range

            if (specParams.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == specParams.CategoryId.Value);
                response.CategoryId = specParams.CategoryId.Value;
            }

            if (specParams.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == specParams.BrandId.Value);
                response.BrandId = specParams.BrandId.Value;
            }

            if (specParams.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= specParams.MinPrice.Value);
                response.MinPrice = specParams.MinPrice.Value;
            }

            if (specParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= specParams.MaxPrice.Value);
                response.MaxPrice = specParams.MaxPrice.Value;
            }

            //3.Sorting

            switch (specParams.Sort)
            {
                case "priceAsc":
                    query = query.OrderBy(p => p.Price);
                    response.Sort = "Price : Low to High";
                    break;
                case "priceDesc":
                    query = query.OrderByDescending(p => p.Price);
                    response.Sort = "Price : High to Low";
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.Created);
                    response.Sort = "Newest Arrivals";
                    break;
                case "featured":
                    query = query.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.Name);
                    response.Sort = "Featured";
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            //4.Pagination

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            response.PageIndex = specParams.PageIndex;
            response.PageSize = specParams.PageSize;
            response.TotalCount = totalCount;
            response.Items = items;

            return response;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id)
                .AsNoTracking()
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Product does not exist.");
            return product;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");

            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            var productToAdd = _mapper.Map<Product>(dto);
            _context.Products.Add(productToAdd);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product added with id = {id}.", productToAdd.Id);
            return _mapper.Map<ProductDto>(productToAdd);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {

            var productToUpdate = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException("Product does not exist.");

            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");

            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            _mapper.Map(dto, productToUpdate);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product updated with id = {id}.", productToUpdate.Id);
            return _mapper.Map<ProductDto>(productToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var productToDelete = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException("Product does not exist.");
            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product deleted with id = {id}.", productToDelete.Id);
        }


    }
}
