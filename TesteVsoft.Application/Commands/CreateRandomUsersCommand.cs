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

public class CreateRandomUsersCommandHandler(IUserRepository userRepository)
    : ICommandHandler<CreateRandomUsersCommand>
{
    public async Task Handle(CreateRandomUsersCommand command, CancellationToken cancellationToken)
    {
        Validate(command);

        var tasks = new List<Task<User>>();

        for (int i = 0; i < command.Amount; i++)
        {
            tasks.Add(Task.Run(() => User.CreateRandom(command.UserNameMask)));
        }

        var users = await Task.WhenAll(tasks);

        await userRepository.AddRangeAsync(users, cancellationToken);
    }

    private void Validate(CreateRandomUsersCommand command)
    {
        if (command.Amount <= 0)
        {
            throw new ValidationException("A quantidade deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(command.UserNameMask))
        {
            throw new ValidationException("A máscara do nome de usuário é obrigatória.");
        }
    }
}