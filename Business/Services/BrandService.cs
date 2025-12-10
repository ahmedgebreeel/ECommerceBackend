using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Brands;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class BrandService(AppDbContext context, IMapper mapper, ILogger<BrandService> logger) : IBrandService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<BrandService> _logger = logger;

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .ProjectTo<BrandDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return brands;
        }

        public async Task<BrandDto> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.Where(x => x.Id == id)
                .AsNoTracking()
                .ProjectTo<BrandDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Brand does not exist.");
            return brand;
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brandToAdd = _mapper.Map<Brand>(dto);
            _context.Brands.Add(brandToAdd);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand added with id = {id}.", brandToAdd.Id);
            return _mapper.Map<BrandDto>(brandToAdd);

        }

        public async Task<BrandDto> UpdateAsync(int id, UpdateBrandDto dto)
        {
            var brandToUpdate = await _context.Brands.FindAsync(id)
                ?? throw new NotFoundException("Brand does not exist");
            _mapper.Map(dto, brandToUpdate);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand updated with id = {id}.", brandToUpdate.Id);
            return _mapper.Map<BrandDto>(brandToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var brandToDelete = await _context.Brands.FindAsync(id)
                ?? throw new NotFoundException("Brand does not exist");
            if (await _context.Products.AnyAsync(p => p.BrandId == id))
                throw new ConflictException("Cannot delete a brand with existing products.");
            _context.Brands.Remove(brandToDelete);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand deleted with id = {id}.", brandToDelete.Id);
        }
    }
}
