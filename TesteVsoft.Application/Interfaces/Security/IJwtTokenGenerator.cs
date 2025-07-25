using TesteVsoft.Domain.Entities;
using TesteVsoft.Application.Dtos;

namespace TesteVsoft.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    JwtTokenDto GenerateToken(User user);
}