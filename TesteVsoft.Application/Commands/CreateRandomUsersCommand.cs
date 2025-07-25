using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using TesteVsoft.Application.Interfaces.BackgroundServices;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Commands;

public class CreateRandomUsersCommand : ICommand
{
    public int Amount { get; set; }
    public string UserNameMask { get; set; } = string.Empty;
}

public class CreateRandomUsersCommandHandler(IBackgroundTaskQueue taskQueue, IUserRepository userRepository) 
    : ICommandHandler<CreateRandomUsersCommand>
{
    public Task Handle(CreateRandomUsersCommand command, CancellationToken cancellationToken)
    {
        Validate(command);

        Enumerable.Range(0, command.Amount).ToList().ForEach(_ =>
        {
            taskQueue.EnqueueAsync(async (userRepo, ct) =>
            {
                await userRepo.AddAsync(User.CreateRandom(command.UserNameMask), ct);
            });
        });

        return Task.CompletedTask;
    }

    private void Validate(CreateRandomUsersCommand command)
    {
        if (command.Amount <= 0)
        {
            throw new ValidationException("A quantidade deve ser maior que zero");
        }

        if (string.IsNullOrWhiteSpace(command.UserNameMask))
        {
            throw new ValidationException("A máscara do nome de usuário é obrigatória");
        }
    }
}