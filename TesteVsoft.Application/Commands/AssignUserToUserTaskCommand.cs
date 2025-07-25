using MassTransit;
using System.Text.Json.Serialization;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record AssignUserToUserTaskCommand : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public required Guid Id { get; set; }
    public User User { get; set; }
}

public class AssignUserToUserTaskCommandHandler(IUserTaskRepository userTaskRepository, IPublishEndpoint publisher)
    : ICommandHandler<AssignUserToUserTaskCommand, UserTask>
{
    public async Task<UserTask> Handle(AssignUserToUserTaskCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.Id, cancellationToken);

        Validate(command, userTask, command.UserId);

        var oldAssignedUser = userTask!.AssignedUser;

        userTask!.AssignToUser(command.User);

        if (oldAssignedUser.Id != command.User.Id)
        {
            await publisher.Publish<UserTaskAssignedToUserDto>(new UserTaskAssignedToUserDto
            {
                UserTaskId = userTask.Id                
            });
        }

        return await userTaskRepository.UpdateAsync(userTask, cancellationToken);
    }

    private static void Validate(AssignUserToUserTaskCommand command, UserTask? userTask, Guid userId)
    {
        if (userTask is null)
        {
            throw new NotFoundException("Tarefa não encontrada");
        }
    }
}