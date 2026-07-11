using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ScriptureMemory.Server.Tools;

public sealed class TokenProvider(IConfiguration config)
{
    public string Create(Admin admin)
    {
        string secretKey = config["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret key not configured");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, admin.AdminEmail ?? throw new ArgumentNullException(nameof(admin.AdminEmail))),
                new Claim("role", admin.Role.ToString() ?? throw new ArgumentNullException(nameof(admin.Role)))
            }),
            Expires = DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:ExpireMinutes")),
            SigningCredentials = credentials,
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public string Create(User user)
    {
        string secretKey = config["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret key not configured");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim("refresh_token_hash", user.Sessions.First().RefreshTokenHash ?? string.Empty),
                new Claim("role", user.Role.ToString() ?? throw new ArgumentNullException(nameof(user.Role)))
            }),
            Expires = DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:UserExpireMinutes")),
            SigningCredentials = credentials,
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
