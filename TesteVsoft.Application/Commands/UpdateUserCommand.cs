using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;

namespace TesteVsoft.Application.Commands;

public class UpdateUserCommand : ICommand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string Email { get; set; } = null;
}

public class UpdateUserCommandHandler(IUserRepository userRepository) : ICommandHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetOneByIdAsync(command.Id, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"Usuário {command.UserName} não encontrado.");
        }

        user.Update(command.Name, command.UserName, command.OldPassword, command.NewPassword, command.Email);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}