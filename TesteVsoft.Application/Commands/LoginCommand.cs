using System.ComponentModel.DataAnnotations;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Interfaces.Security;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record LoginCommand(string UserName, string Password) : ICommand;

public class LoginCommandHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IUserRepository userRepository) : ICommandHandler<LoginCommand, JwtTokenDto>
{
    public async Task<JwtTokenDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        Validate(command);
        
        var filter = new FilterBuilder<User, Guid>()
            .AddWhere(u => u.UserName, WhereOperationTypes.Equal, command.UserName)
            .Build();

        var user = await userRepository.GetOneAsync(filter, cancellationToken);

        if (user is null || !user.Login(command.Password))
        {
            throw new UnauthorizedAccessException("Nome de usuário ou senha incorretas");
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        return jwtTokenGenerator.GenerateToken(user);
    }

    private static void Validate(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.UserName))
        {
            throw new ValidationException("O nome de usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            throw new ValidationException("A senha é obrigatória.");
        }
    }
}
