using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Brands;
using MyApp.API.Entities;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class BrandService(AppDbContext context, IMapper mapper) : IBrandService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            return await _context.Brands.ProjectTo<BrandDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            return await _context.Brands.Where(x => x.Id == id)
                .ProjectTo<BrandDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = _mapper.Map<Brand>(dto);
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return _mapper.Map<BrandDto>(brand);

        }

        public async Task<BrandDto?> UpdateAsync(int id, UpdateBrandDto dto)
        {
            var brandToUpdate = await _context.Brands.FindAsync(id);
            if (brandToUpdate is null)
                return null;
            _mapper.Map(dto, brandToUpdate);
            await _context.SaveChangesAsync();
            return _mapper.Map<BrandDto>(brandToUpdate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brandToDelete = await _context.Brands.FindAsync(id);
            if (brandToDelete is null)
                return false;
            _context.Brands.Remove(brandToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
