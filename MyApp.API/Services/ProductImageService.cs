using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.ProductImages;
using MyApp.API.Entities;
using MyApp.API.Exceptions;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class ProductImageService(AppDbContext context, IMapper mapper, ILogger<ProductImageService> logger) : IProductImageService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductImageService> _logger = logger;

        public async Task<IEnumerable<ProductImageDto>> GetAllAsync(int productId)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new NotFoundException("Product does not exist.");
            return await _context.ProductImages
                .AsNoTracking()
                .Where(pi => pi.ProductId == productId)
                .ProjectTo<ProductImageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProductImageDto> AddImageAsync(int productId, AddProductImageDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new NotFoundException("Product does not exist.");

            var productImageToAdd = _mapper.Map<ProductImage>(dto);
            productImageToAdd.ProductId = productId;
            _context.ProductImages.Add(productImageToAdd);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image added with id = {imageId} for product = {productId}.", productImageToAdd.Id, productImageToAdd.ProductId);
            return _mapper.Map<ProductImageDto>(productImageToAdd);

        }

        public async Task SetMainImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productIdFromRoute)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            var currentMainImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == product.Id && pi.IsMain);

            if (currentMainImage?.Id == imageId)
                return;

            if (currentMainImage != null)
            {
                currentMainImage.IsMain = false;
                await _context.SaveChangesAsync();
            }
            image.IsMain = true;

            product.ImageUrl = image.ImageUrl;

            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image with id = {imageId} for product = {productId} is set as main.", image.Id, image.ProductId);

        }

        public async Task DeleteImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productIdFromRoute)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            //Throw ConflictException
            if (image.IsMain)
                throw new ConflictException("Cannot delete the main image. Set another image as main first.");

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image deleted with id = {imageId} for product = {productId}.", image.Id, image.ProductId);
        }
    }
}
