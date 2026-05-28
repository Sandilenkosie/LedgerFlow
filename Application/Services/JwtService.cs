using Domain.Entities;
using Application.ViewModels;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthenticationResult GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new Exception("JWT key not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "LedgerFlow";
        var audience = _configuration["Jwt:Audience"] ?? "LedgerFlowUsers";
        var expiry = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", user.Id.ToString())
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiry);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthenticationResult { Token = tokenStr, ExpiresAt = expiresAt };
    }
}
