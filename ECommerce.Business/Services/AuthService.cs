using AutoMapper;
using ECommerce.Business.DTOs.Auth.Requests;
using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        IMapper mapper, ILogger<AuthService> logger,
        ITokenService tokenService,
        AppDbContext context) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly ITokenService _tokenService = tokenService;
        private readonly AppDbContext _context = context;

        public async Task<(AuthResponse, string, DateTime)> RegisterAsync(RegisterRequest registerRequest)
        {
            var userToCreate = _mapper.Map<ApplicationUser>(registerRequest);

            var result = await _userManager.CreateAsync(userToCreate, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new ConflictException(errors);
            }

            await _userManager.AddToRoleAsync(userToCreate, "Customer");
            var roles = await _userManager.GetRolesAsync(userToCreate);


            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User created with id = {id}.", userToCreate.Id);

            //1.Generate JWT Token (Access Token)
            var accessToken = _tokenService.CreateAccessToken(userToCreate, roles);

            // 2. Generate Refresh Token (Based on RememberMe)
            var refreshToken = _tokenService.GenerateRefreshToken(userToCreate.Id, true);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {Identifier} logged in.", userToCreate.Email);

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                UserId = userToCreate.Id,
                FullName = $"{userToCreate.FirstName} {userToCreate.LastName}",
                Email = userToCreate.Email!,
                Roles = roles,
                AvatarUrl = userToCreate.AvatarUrl,
            };
            return (authResponse, refreshToken.Token, refreshToken.ExpiresOn);
        }

        public async Task<(AuthResponse, string, DateTime)> LoginAsync(LoginRequest loginRequest)
        {
            //Authenticate User from databse
            ApplicationUser? user;
            if (loginRequest.Identifier.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(loginRequest.Identifier);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginRequest.Identifier);
            }

            if (user is null)
                throw new UnauthorizedException("User not found, please register.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Account is locked. Try again in 15 minutes.");


            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            // Lockout ( If password entered invalid many time, lock the user out)
            if (!isValidPassword)
            {
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                    throw new UnauthorizedException("Account locked due to too many failed attempts.");

                throw new UnauthorizedException("Incorrect email or password.");
            }

            //Reset Failed Access Counter on Successful login.
            await _userManager.ResetAccessFailedCountAsync(user);

            //1.Generate JWT Token (Access Token)
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            // 2. Generate Refresh Token (Based on RememberMe)
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id, loginRequest.RememberMe);
            //Check if there are old revoked tokens and clean them up
            var junkTokens = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id && (t.ExpiresOn <= DateTime.UtcNow || t.RevokedOn != null))
                .ToListAsync();
            if (junkTokens.Count != 0)
            {
                _context.RefreshTokens.RemoveRange(junkTokens);
            }

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {Identifier} logged in.", user.Email);

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                Roles = roles,
                AvatarUrl = user.AvatarUrl,
            };
            return (authResponse, refreshToken.Token, refreshToken.ExpiresOn);
        }

        public async Task<(AuthResponse, string, DateTime)> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken)
                ?? throw new UnauthorizedException("Invalid token.");

            if (!existingToken.IsActive)
            {

                await _context.RefreshTokens
                    .Where(t => t.UserId == existingToken.UserId)
                    .ExecuteUpdateAsync(u => u.SetProperty(t => t.RevokedOn, DateTime.UtcNow));

                throw new UnauthorizedException("Invalid token.");
            }

            existingToken.RevokedOn = DateTime.UtcNow;

            bool isLongLived = (existingToken.ExpiresOn - existingToken.Created).TotalDays > 7;

            var newRefreshToken = _tokenService.GenerateRefreshToken(existingToken.UserId, isLongLived);

            var roles = await _userManager.GetRolesAsync(existingToken.User);
            var newAccessToken = _tokenService.CreateAccessToken(existingToken.User, roles);

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var authResponse = new AuthResponse
            {
                AccessToken = newAccessToken,
                UserId = existingToken.User.Id,
                FullName = $"{existingToken.User.FirstName} {existingToken.User.LastName}",
                Email = existingToken.User.Email!,
                Roles = roles,
                AvatarUrl = existingToken.User.AvatarUrl,
            };
            return (authResponse, newRefreshToken.Token, newRefreshToken.ExpiresOn);


        }

        public async Task RevokeTokenAsync(string token)
        {
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (existingToken == null)
                throw new NotFoundException("Token not found.");

            existingToken.RevokedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Token revoked for user {userId}.", existingToken.UserId);
        }
    }
}
