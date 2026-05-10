using HouseBroker.App.Auth.D;
using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HouseBroker.Infra.Identity;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly JWTConfiguration _jwtCOnfifg;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService tokenService,
        IOptions<JWTConfiguration> jwtCOnfifg)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtCOnfifg = jwtCOnfifg.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("Email is already registered.");

        // not using Mapperly here as there are IdentityUser properties to ignore for 3 fields
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Role = dto.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtCOnfifg.ExpiryMinutes)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email");

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
            throw new UnauthorizedAccessException("Incorrect password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtCOnfifg.ExpiryMinutes)
        };
    }
}