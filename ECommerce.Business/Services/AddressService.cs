using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class AddressService(AppDbContext context,
        IMapper mapper,
        ILogger<BrandService> logger,
        IHttpContextAccessor httpContext) : IAddressService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<BrandService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        //Admin
        //1. userId null => return all saved addresses for all users as IEnumerable<AddressWithUserDto>
        //2. userId not null => return all saved addresses for user with id = userId as IEnumerable<AddressDto>

        //Not Admin (Customer)
        // userId null or not => return all saved addresses for current logged in user as IEnumerable<AddressDto>

        public async Task<IEnumerable<AddressDto>> GetAllAsync(string? userId = null)
        {
            var currentUserId = GetCurrentUserId();

            var query = _context.Addresses.AsNoTracking().AsQueryable();

            if (!IsAdmin())
            {
                query = query.Where(a => a.UserId == currentUserId);
            }
            else
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(a => a.UserId == userId);
                }
            }

            if (IsAdmin() && string.IsNullOrEmpty(userId))
                return await query
                    .ProjectTo<AddressWithUserDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            else
                return await query
                    .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

        }

        public async Task<AddressDto> GetByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();

            var address = await _context.Addresses
                .AsNoTracking()
                .Where(a => (IsAdmin() || a.UserId == currentUserId) && a.Id == id)
                .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Address does not exist.");
            return address;
        }

        public async Task<AddressDto> CreateAsync(CreateAddressDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var addressToCreate = _mapper.Map<Address>(dto);
            addressToCreate.UserId = currentUserId;

            _context.Addresses.Add(addressToCreate);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
                _logger.LogInformation("Address created with id = {id} for user {userId}", addressToCreate.Id, addressToCreate.UserId);
            return _mapper.Map<AddressDto>(addressToCreate);
        }

        public async Task<AddressDto> UpdateAsync(int id, UpdateAddressDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var addressToUpdate = await _context.Addresses.
                FirstOrDefaultAsync(a => a.Id == id && a.UserId == currentUserId)
                ?? throw new NotFoundException("Address does not exist.");

            _mapper.Map(dto, addressToUpdate);

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
                _logger.LogInformation("updated with id = {id} for user {userId}", addressToUpdate.Id, addressToUpdate.UserId);

            return _mapper.Map<AddressDto>(addressToUpdate);

        }

        public async Task DeleteAsync(int id)
        {
            var currentUserId = GetCurrentUserId();

            var addressToDelete = await _context.Addresses.
                FirstOrDefaultAsync(a => a.Id == id && (IsAdmin() || a.UserId == currentUserId))
                ?? throw new NotFoundException("Address does not exist.");

            _context.Addresses.Remove(addressToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Address deleted with id = {id}", id);
        }


        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        private bool IsAdmin() => _httpContext?.HttpContext?.User.IsInRole("Admin") ?? false;

    }
}
