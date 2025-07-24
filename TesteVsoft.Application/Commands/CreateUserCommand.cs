using System.ComponentModel.DataAnnotations;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record CreateUserCommand(string Name, string UserName, string Password, string Email) 
    : ICommand;

public class CreateUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<CreateUserCommand, User>
{
    public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var existingUserFilter = new FilterBuilder<User, Guid>()
            .AddWhere(x => x.UserName, WhereOperationTypes.Equal, command.UserName)
            .Build();
        
        var existingUser = await userRepository.GetOneAsync(existingUserFilter, cancellationToken);

        if (existingUser is not null)
        {
            throw new ValidationException($"O nome de usuário '{command.UserName}' já está em uso.");
        }

        var user = User.Create(command.Name, command.UserName, command.Password, command.Email);

        return await userRepository.AddAsync(user, cancellationToken);
    }
}