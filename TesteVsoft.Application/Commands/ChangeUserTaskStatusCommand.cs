using System.Text.Json.Serialization;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Enums;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace TesteVsoft.Application.Commands;

public record ChangeUserTaskStatusCommand : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public required Guid Id { get; set; }
    public required UserTaskStatusTypes Status { get; set; }
}

public class ChangeUserTaskStatusCommandHandler(IUserTaskRepository userTaskRepository) : ICommandHandler<ChangeUserTaskStatusCommand, UserTask>
{
    public async Task<UserTask> Handle(ChangeUserTaskStatusCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.Id, cancellationToken);

        Validate(command, userTask, command.UserId);
        userTask!.ChangeStatus(command.Status);

        return await userTaskRepository.UpdateAsync(userTask, cancellationToken);
    }

    private static void Validate(ChangeUserTaskStatusCommand command, UserTask? userTask, Guid userId)
    {
        if (userId.Equals(Guid.Empty))
        {
            throw new UnauthorizedAccessException();
        }

        if (userTask is null)
        {
            throw new NotFoundException("Tarefa não encontrada");
        }
    }
}