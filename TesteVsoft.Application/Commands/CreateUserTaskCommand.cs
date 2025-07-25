using MassTransit;
using System.Text.Json.Serialization;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record CreateUserTaskCommand : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime DueDate { get; set; }
}

public class CreateUserTaskCommandHandler(IUserRepository userRepository, IUserTaskRepository userTaskRepository, IPublishEndpoint publisher)
    : ICommandHandler<CreateUserTaskCommand, UserTask>
{
    public async Task<UserTask> Handle(CreateUserTaskCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetOneByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        var userTask = UserTask.Create(command.Title, command.Description, command.DueDate, user!);

        var creteadUserTask = await userTaskRepository.AddAsync(userTask, cancellationToken);

        await publisher.Publish<UserTaskAssignedToUserDto>(new UserTaskAssignedToUserDto
        {
            UserTaskId = creteadUserTask.Id
        });

        return creteadUserTask;
    }
}