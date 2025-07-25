using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.Security;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Security;

[Scoped]
public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public JwtTokenDto GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            audience: configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(int.Parse(configuration["Jwt:Expiration"]!)),
            issuer: configuration["Jwt:Issuer"],
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtTokenDto(
           AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
           ExpiresIn: (int)TimeSpan.FromHours(int.Parse(configuration["Jwt:Expiration"]!)).TotalSeconds,
           UserId: user.Id,
           UserName: user.UserName,
           Name: user.Name
        );
    }
}
