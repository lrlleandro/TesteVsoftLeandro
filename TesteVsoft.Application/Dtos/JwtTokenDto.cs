namespace TesteVsoft.Application.Dtos;

public record JwtTokenDto(
    string AccessToken,
    int ExpiresIn,
    Guid UserId,
    string UserName,
    string Name
);