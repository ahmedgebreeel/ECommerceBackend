using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Addresses.Requests;
using ECommerce.Business.DTOs.Addresses.Responses;
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

        public async Task<IEnumerable<AddressSummaryDto>> GetAllAddressesAsync()
        {
            var currentUserId = GetCurrentUserId();

            var addresses = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == currentUserId)
                .OrderByDescending(a => a.Updated)
                .ProjectTo<AddressSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return addresses;
        }

        public async Task<AddressSummaryDto> CreateAddressAsync(CreateAddressRequest dto)
        {
            var currentUserId = GetCurrentUserId();

            var addressToCreate = _mapper.Map<Address>(dto);
            addressToCreate.UserId = currentUserId;
            addressToCreate.District ??= addressToCreate.City;
            addressToCreate.Governorate ??= addressToCreate.City;
            addressToCreate.Label ??= "No Label";

            if (!await _context.Addresses.AnyAsync(a => a.UserId == currentUserId)) //addresses is empty for the logged in user
                addressToCreate.IsDefault = true;

            _context.Addresses.Add(addressToCreate);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {currentUserId} added new address with id = {adddressToCreateId}", currentUserId, addressToCreate.Id);

            return _mapper.Map<AddressSummaryDto>(addressToCreate);

        }

        public async Task<AddressSummaryDto> UpdateAddressAsync(int addressId, UpdateAddressRequest dto)
        {
            var currentUserId = GetCurrentUserId();

            var addressToUpdate = await _context.Addresses
                .Where(a => a.Id == addressId && a.UserId == currentUserId)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Address does not exist or does not belong to current user.");

            _mapper.Map(dto, addressToUpdate);
            addressToUpdate.District ??= addressToUpdate.City;
            addressToUpdate.Governorate ??= addressToUpdate.City;
            addressToUpdate.Label ??= "No Label";

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {currentUserId} updated his existing address with id = {adddressToCreateId}", currentUserId, addressToUpdate.Id);

            return _mapper.Map<AddressSummaryDto>(addressToUpdate);
        }

        public async Task<IEnumerable<AddressSummaryDto>> SetDefaultAsync(int addressId)
        {
            var currentUserId = GetCurrentUserId();

            var savedAddresses = await _context.Addresses.Where(a => a.UserId == currentUserId).ToListAsync();

            var addressToMarkDefault = savedAddresses.Where(a => a.Id == addressId).FirstOrDefault()
                ?? throw new NotFoundException("Address does not exist or does not belong to current user.");


            if (!addressToMarkDefault.IsDefault)
            {
                var currentDefaultAddress = savedAddresses
                    .Where(a => a.IsDefault == true)
                    .FirstOrDefault();

                using var transaction = _context.Database.BeginTransaction();
                if (currentDefaultAddress is not null)
                {
                    currentDefaultAddress.IsDefault = false;
                    await _context.SaveChangesAsync();
                }
                addressToMarkDefault.IsDefault = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("User {currentUserId} changed his default address from address : {currentDefaultAddressId} to address : {addressToMarkDefaultId}", currentUserId, currentDefaultAddress?.Id, addressToMarkDefault.Id);
            }

            return _mapper.Map<IEnumerable<AddressSummaryDto>>(savedAddresses);

        }

        public async Task DeleteAddressAsync(int addressId)
        {
            var currentUserId = GetCurrentUserId();
            var addressToDelete = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == currentUserId)
                ?? throw new NotFoundException("Address does not exist or does not belong to current user.");

            //if (addressToDelete.IsDefault)
            //    throw new ConflictException("Cannot delete the default address, set another to default first.");

            _context.Addresses.Remove(addressToDelete);

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {currentUserId} removed his saved address with id = {addressToDelteId}", currentUserId, addressToDelete.Id);
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }
    }
}
