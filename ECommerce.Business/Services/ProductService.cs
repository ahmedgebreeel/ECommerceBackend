using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Products;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
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

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return products;
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
