using AutoMapper;
using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<AuthService> logger, ITokenService tokenService) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var userToCreate = _mapper.Map<ApplicationUser>(dto);
            var result = await _userManager.CreateAsync(userToCreate, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ConflictException($"Could not register the user ,Errors : {errors}");
            }
            await _userManager.AddToRoleAsync(userToCreate, "Customer");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User created with id = {id}.", userToCreate.Id);

            return _mapper.Map<UserDto>(userToCreate);

        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            ApplicationUser? user;
            if (dto.Identifier.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(dto.Identifier);
            }
            else
            {
                user = await _userManager.FindByNameAsync(dto.Identifier);
            }

            if (user is null)
                throw new UnauthorizedException("Invalid username/email.");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isValidPassword)
                throw new UnauthorizedException("Invalid password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {Identifier} logged in.", dto.Identifier);

            return token;
        }
    }
}
