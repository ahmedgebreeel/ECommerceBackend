using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Categories;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class CategoryService(AppDbContext context, IMapper mapper, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CategoryService> _logger = logger;

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return categories;
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _context.Categories.Where(x => x.Id == id)
                .AsNoTracking()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Category does not exist.");
            return category;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var categoryToAdd = _mapper.Map<Category>(dto);
            _context.Categories.Add(categoryToAdd);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category added with id = {id}.", categoryToAdd.Id);
            return _mapper.Map<CategoryDto>(categoryToAdd);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(id)
                ?? throw new NotFoundException("Category does not exist.");
            _mapper.Map(dto, categoryToUpdate);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category updated with id = {id}.", categoryToUpdate.Id);
            return _mapper.Map<CategoryDto>(categoryToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var categoryToDelete = await _context.Categories.FindAsync(id)
                ?? throw new NotFoundException("Category does not exist.");
            if (await _context.Products.AnyAsync(p => p.CategoryId == id))
                throw new ConflictException("Cannot delete a Category with existing products.");
            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category deleted with id = {id}.", categoryToDelete.Id);

        }
    }
}
