using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Categories.Admin;
using ECommerce.Business.DTOs.Categories.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Categories;
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

        public async Task<PagedResponseDto<AdminCategoryDto>> GetAllCategoriesAdminAsync(AdminCategorySpecParams specParams)
        {
            var query = _context.Categories.AsNoTracking().AsQueryable();

            //Default Sort
            query = query.OrderBy(c => c.Id);

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(c => (c.Name.Contains(specParams.Search))
                || (c.Description != null && c.Description.Contains(specParams.Search)));
            }

            //Pagination
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<AdminCategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponseDto<AdminCategoryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<AdminCategoryDetailsDto> GetCategoryAdminAsync(int categoryId)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .ProjectTo<AdminCategoryDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Category does not exist.");
            return category;

        }

        public async Task<AdminCategoryDetailsDto> CreateCategoryAdminAsync(AdminCreateCategoryDto dto)
        {
            string? parentHierarchyPath = null;

            //validate parent category
            if (dto.ParentId.HasValue)
            {
                var parentCateory = await _context.Categories
                    .AsNoTracking()
                    .Where(c => c.Id == dto.ParentId.Value)
                    .Select(c => new
                    {
                        c.Name,
                        c.HierarchyPath
                    })
                    .FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Parent Category does not exist.");

                parentHierarchyPath = parentCateory.HierarchyPath;
            }

            var categoryToCreate = _mapper.Map<Category>(dto);
            categoryToCreate.HierarchyPath = parentHierarchyPath is null ? categoryToCreate.Name : $"{parentHierarchyPath}\\{categoryToCreate.Name}";

            _context.Categories.Add(categoryToCreate);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category added with id = {id}.", categoryToCreate.Id);

            return _mapper.Map<AdminCategoryDetailsDto>(categoryToCreate);
        }

        public async Task<AdminCategoryDetailsDto> UpdateCategoryAdminAsync(int categoryId, AdminUpdateCategoryDto dto)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(categoryId)
                ?? throw new NotFoundException("Category does not exist.");

            string? parentHierarchyPath = null;
            //validate parent category
            if (dto.ParentId.HasValue)
            {
                var parentCateory = await _context.Categories
                    .AsNoTracking()
                    .Where(c => c.Id == dto.ParentId.Value)
                    .Select(c => new
                    {
                        c.Name,
                        c.HierarchyPath
                    })
                    .FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Parent Category does not exist.");

                parentHierarchyPath = parentCateory.HierarchyPath;
            }
            _mapper.Map(dto, categoryToUpdate);
            categoryToUpdate.HierarchyPath = parentHierarchyPath is null ? categoryToUpdate.Name : $"{parentHierarchyPath}\\{categoryToUpdate.Name}";
            categoryToUpdate.Updated = DateTime.UtcNow;

            //updating children of this category
            await _context.Categories.Where(c => c.ParentId == categoryToUpdate.Id).ForEachAsync(c =>
            {
                c.HierarchyPath = $"{categoryToUpdate.HierarchyPath}\\{c.Name}";
                c.Updated = DateTime.UtcNow;
            });

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category updated with id = {id}.", categoryToUpdate.Id);

            return _mapper.Map<AdminCategoryDetailsDto>(categoryToUpdate);
        }

        public async Task DeleteCategoryAdminAsync(int categoryId)
        {
            var categoryToDelete = await _context.Categories.FindAsync(categoryId)
                ?? throw new NotFoundException("Category does not exist");

            //check if category has subcategories
            var hasSubcategories = await _context.Categories.AnyAsync(c => c.ParentId == categoryId);
            if (hasSubcategories)
                throw new ConflictException("Cannot delete a category having children subcategories.");

            //check if category is terminal having products
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
            if (hasProducts)
                throw new ConflictException("Cannot delete a category having products.");

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Category deleted with id = {id}.", categoryId);
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var lookup = categories.ToDictionary(c => c.Id);

            var rootCategories = new List<CategoryDto>();

            foreach (var category in categories)
            {
                if (category.ParentId.HasValue && lookup.TryGetValue(category.ParentId.Value, out var parent))
                    parent.Subcategories.Add(category);
                else
                    rootCategories.Add(category);
            }

            return rootCategories;
        }
    }
}
