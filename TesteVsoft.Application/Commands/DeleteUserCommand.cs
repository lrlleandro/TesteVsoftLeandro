using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;

namespace TesteVsoft.Application.Commands;

public class DeleteUserCommand : ICommand
{
    public DeleteUserCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}

public class DeleteUserCommandHandler(IUserRepository userRepository) : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetOneByIdAsync(command.Id, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"Usuário não encontrado.");
        }

        await userRepository.RemoveAsync(user, cancellationToken);
    }
}