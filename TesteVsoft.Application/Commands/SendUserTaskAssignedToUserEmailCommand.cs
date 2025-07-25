using Microsoft.Extensions.Logging;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public record SendUserTaskAssignedToUserEmailCommand(Guid UserTaskId) : ICommand;

public class SendUserTaskAssignedToUserCommandHandler(ILogger<SendUserTaskAssignedToUserCommandHandler> logger, IUserTaskRepository userTaskRepository, IUserRepository userRepository, IEmailTemplateService emailTemplateService, IEmailService emailService) : ICommandHandler<SendUserTaskAssignedToUserEmailCommand>
{
    public async Task Handle(SendUserTaskAssignedToUserEmailCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.UserTaskId, cancellationToken);
        var assignedUser = await userRepository.GetOneByIdAsync(userTask.AssignedUserId, cancellationToken);

        if (userTask is null)
        {
            logger.LogError($"Tarefa não encontrada: {command.UserTaskId}");
            return;
        }

        if (assignedUser is null)
        {
            logger.LogError($"Usuário não encontrado: {userTask.AssignedUserId}");
            return;
        }

        if (string.IsNullOrEmpty(assignedUser?.Email?.Address ?? string.Empty))
        {
            logger.LogWarning($"Usuário {assignedUser.Name} não possui e-mail cadastrado.");
            return;
        }

        var emailBody = await emailTemplateService.GenerateAsync(userTask);
        await emailService.SendAsync(assignedUser!.Email.Address, "VSoft - Nova Tarefa Atribuída", emailBody);
    }
}