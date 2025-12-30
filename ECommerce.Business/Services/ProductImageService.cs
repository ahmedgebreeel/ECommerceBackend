using AutoMapper;
using ECommerce.Business.DTOs.ProductImages.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class ProductImageService(
        AppDbContext context,
        IMapper mapper,
        ILogger<ProductImageService> logger,
        IFileStorageService fileStorageService) : IProductImageService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductImageService> _logger = logger;
        private readonly IFileStorageService _fileStorageService = fileStorageService;



        public async Task<IEnumerable<ProductImageDto>> AddImagesAsync(int productId, List<IFormFile> files)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
            {
                throw new NotFoundException("Product does not exist.");
            }


            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Starting upload of {Count} images for Product {ProductId}",
                    files.Count,
                    productId);
            }

            var uploadedImages = new List<ProductImage>();
            var uploadedFileNames = new List<string>();
            try
            {
                foreach (var file in files)
                {

                    var relativePath = await _fileStorageService.SaveFileAsync(file, "products");
                    var fileName = Path.GetFileName(relativePath);

                    var productImageToAdd = new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = relativePath,
                        IsMain = false
                    };

                    uploadedImages.Add(productImageToAdd);
                    uploadedFileNames.Add(fileName);

                }

                _context.ProductImages.AddRange(uploadedImages);
                await _context.SaveChangesAsync();

            }

            catch
            {
                if (uploadedFileNames.Count > 0)
                {
                    foreach (var fileName in uploadedFileNames)
                        await _fileStorageService.DeleteFileAsync($"/images/products/{fileName}");
                }
                throw;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Successfully added {Count} images to Product {ProductId}. Generated Files: {FileNames}",
                    uploadedImages.Count,
                    productId,
                    string.Join(", ", uploadedFileNames));
            }

            return _mapper.Map<IEnumerable<ProductImageDto>>(uploadedImages);

        }

        public async Task SetMainImageAsync(int productId, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productId)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            var currentMainImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == product.Id && pi.IsMain);

            if (currentMainImage?.Id == imageId)
                return;

            using var transaction = _context.Database.BeginTransaction();
            if (currentMainImage != null)
            {
                currentMainImage.IsMain = false;
                await _context.SaveChangesAsync();
            }
            image.IsMain = true;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image with id = {imageId} for product = {productId} is set as main.", image.Id, image.ProductId);

        }

        public async Task DeleteImageAsync(int productId, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productId)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            //Throw ConflictException
            if (image.IsMain)
                throw new ConflictException("Cannot delete the main image. Set another image as main first.");

            _context.ProductImages.Remove(image);
            //await _fileStorageService.DeleteFileAsync(image.ImageUrl);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image deleted with id = {imageId} for product = {productId}.", image.Id, image.ProductId);
        }
    }
}
