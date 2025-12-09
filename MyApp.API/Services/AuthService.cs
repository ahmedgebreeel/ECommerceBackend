using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyApp.API.DTOs.Auth;
using MyApp.API.Entities;
using MyApp.API.Exceptions;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
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
                throw new BadRequestException($"Could not register the user ,Errors : {errors}");
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User created with id = {id}.", userToCreate.Id);

            return _mapper.Map<UserDto>(userToCreate);

        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user = await _userManager.FindByEmailAsync(dto.Email);
            else if (!string.IsNullOrWhiteSpace(dto.UserName))
                user = await _userManager.FindByNameAsync(dto.UserName);
            else
            {
                throw new BadRequestException("Either username or email must be provided.");
            }

            if (user is null)
                throw new UnauthorizedException("Invalid username/email.");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isValidPassword)
                throw new UnauthorizedException("Invalid password.");

            var token = _tokenService.CreateToken(user);

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("User {Identifier} logged in.", dto.Email ?? dto.UserName);

            return token;
        }
    }
}
