using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;

namespace TesteVsoft.Application.Commands;

public class DeleteUserTaskCommand : ICommand
{
    public DeleteUserTaskCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}

public class DeleteUserTaskCommandHandler(IUserTaskRepository usertaskRepository) : ICommandHandler<DeleteUserTaskCommand>
{
    public async Task Handle(DeleteUserTaskCommand command, CancellationToken cancellationToken)
    {
        var usertask = await usertaskRepository.GetOneByIdAsync(command.Id, cancellationToken);

        if (usertask is null)
        {
            throw new NotFoundException($"Tarefa não encontrada.");
        }

        await usertaskRepository.RemoveAsync(usertask, cancellationToken);
    }
}