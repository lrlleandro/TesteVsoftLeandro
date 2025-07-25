using MassTransit;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Enums;

namespace TesteVsoft.Application.Commands;

public record UpdateUserTaskCommand : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime DueDate { get; set; }
    public UserTaskStatusTypes Status { get; set; }
    public Guid AssignedUserId { get; set; }
}

public class UpdateUserTaskCommandHandler(IUserTaskRepository userTaskRepository, IUserRepository userRepository, IPublishEndpoint publisher)
    : ICommandHandler<UpdateUserTaskCommand, UserTask>
{
    public async Task<UserTask> Handle(UpdateUserTaskCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.Id, cancellationToken);

        Validate(command, userTask, command.UserId);

        var assignedUser = await userRepository.GetOneByIdAsync(command.AssignedUserId, cancellationToken);

        if (assignedUser is null)
        {
            throw new NotFoundException("Usuário atribuído não encontrado");
        }

        var oldAssignedUser = await userRepository.GetOneByIdAsync(userTask.AssignedUserId, cancellationToken);

        userTask!.Update(command.Title, command.Description, command.DueDate, command.Status, assignedUser);

        if (oldAssignedUser!.Id != assignedUser.Id)
        {
            await publisher.Publish<UserTaskAssignedToUserDto>(new UserTaskAssignedToUserDto
            {
                UserTaskId = userTask.Id
            });
        }

        return await userTaskRepository.UpdateAsync(userTask, cancellationToken);
    }

    private static void Validate(UpdateUserTaskCommand command, UserTask? userTask, Guid userId)
    {
        if (userTask is null)
        {
            throw new NotFoundException("Tarefa não encontrada");
        }

        if (string.IsNullOrEmpty(command.Title))
        {
            throw new ValidationException("O título da tarefa é obrigatório");
        }

        if (string.IsNullOrEmpty(command.Description))
        {
            throw new ValidationException("A descrição da tarefa é obrigatória");
        }
    }
}