using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Users.Requests;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Users;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class UserService(AppDbContext context,
        IMapper mapper,
        ILogger<UserService> logger,
        IHttpContextAccessor httpContext,
        IFileStorageService fileStorageService,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        //Customers
        public async Task<UserDetailsResponse> GetDetailsAsync()
        {
            var currentUserId = GetCurrentUserId();
            var userDetails = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectTo<UserDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("User does not exist");
            return userDetails;

        }

        public async Task<UserDetailsResponse> UpdateImageAsync(UploadImageRequest uploadImageRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            //remove old image if found
            if (user.AvatarUrl is not null)
            {
                await _fileStorageService.DeleteFileAsync(user.AvatarUrl);
            }

            //upload new image
            var relativePath = await _fileStorageService.SaveFileAsync(uploadImageRequest.File, "users");
            user.AvatarUrl = relativePath;
            await _context.SaveChangesAsync();

            var fileName = Path.GetFileName(relativePath);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {userID} successfully add a new profile image with name {fileName}",
                    currentUserId,
                    fileName);
            }

            return _mapper.Map<UserDetailsResponse>(user);
        }

        public async Task DeleteImageAsync()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");


            if (user.AvatarUrl is null)
                throw new NotFoundException("Profile Image does not exist.");

            var filePath = user.AvatarUrl;
            var fileName = Path.GetFileName(filePath);
            user.AvatarUrl = null;
            await _fileStorageService.DeleteFileAsync(filePath);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {userID} removed his existing profile image with name {fileName}",
                    currentUserId,
                    fileName);
            }

        }

        public async Task<AuthResponse> UpdatePersonalInfoAsync(UpdateUserRequest updateUserRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            // 1. Update Basic Fields
            user.FirstName = updateUserRequest.FirstName;
            user.LastName = updateUserRequest.LastName;
            user.PhoneNumber = updateUserRequest.PhoneNumber;


            if (updateUserRequest.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(updateUserRequest.Email);
                if (emailExists != null && emailExists.Id != currentUserId)
                    throw new ConflictException("Email is already in use.");

                user.Email = updateUserRequest.Email;
            }

            if (updateUserRequest.UserName != user.UserName)
            {
                var userNameExists = await _userManager.FindByNameAsync(updateUserRequest.UserName);
                if (userNameExists != null && userNameExists.Id != currentUserId)
                    throw new ConflictException("Username is already taken.");

                user.UserName = updateUserRequest.UserName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new ConflictException(errors);
            }


            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);

            return new AuthResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                Roles = roles,
                AvatarUrl = user.AvatarUrl,
            };
        }

        public async Task<(AuthResponse, string, DateTime)> UpdatePasswordAsync(UpdatePasswordRequest updatePasswordRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordRequest.OldPassword, updatePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new BadRequestException(errors);
            }

            var userTokens = await _context.RefreshTokens.Where(t => t.UserId == currentUserId).ToListAsync();
            var existingToken = userTokens.OrderByDescending(t => t.Created).FirstOrDefault();
            bool isLongLived = false;

            if (existingToken != null)
            {
                isLongLived = (existingToken.ExpiresOn - existingToken.Created).TotalDays > 7;
            }

            if (userTokens.Count != 0)
            {
                _context.RefreshTokens.RemoveRange(userTokens);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.CreateAccessToken(user, roles);

            var refreshToken = _tokenService.GenerateRefreshToken(currentUserId, isLongLived);

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();

            // 5. Create Response Object
            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                Roles = roles,
                AvatarUrl = user.AvatarUrl
            };

            // 6. Return Tuple
            return (authResponse, refreshToken.Token, refreshToken.ExpiresOn);
        }

        //Admin

        public Task<PagedResponse<AdminUserSummaryDto>> GetAllUsersAdminAsync(AdminUserSpecParams specParams)
        {
            throw new NotImplementedException();
        }

        public Task<AdminUserDetailsResponse> GetUserDetailsAdminAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AdminUserDetailsResponse> UpdateUserRoleAdminAsync(string userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAdminAsync(string userId)
        {
            throw new NotImplementedException();
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
