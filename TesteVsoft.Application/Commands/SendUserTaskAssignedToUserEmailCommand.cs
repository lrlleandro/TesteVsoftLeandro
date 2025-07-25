using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record SendUserTaskAssignedToUserEmailCommand(Guid UserTaskId) : ICommand;

public class SendUserTaskAssignedToUserCommandHandler(IUserTaskRepository userTaskRepository, IUserRepository userRepository, IEmailTemplateService emailTemplateService, IEmailService emailService) : ICommandHandler<SendUserTaskAssignedToUserEmailCommand>
{
    public async Task Handle(SendUserTaskAssignedToUserEmailCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.UserTaskId, cancellationToken);
        var assignedUser = await userRepository.GetOneByIdAsync(userTask.AssignedUserId, cancellationToken);

        if (userTask is null)
        {
            throw new InvalidOperationException($"Tarefa não encontrada: {command.UserTaskId}");
        }

        var emailBody = await emailTemplateService.GenerateAsync(userTask);
        await emailService.SendAsync(assignedUser!.Email.Address, "VSoft - Nova Tarefa Atribuída", emailBody);
    }
}