using System.Text;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Interfaces.Services;

namespace TesteVsoft.Application.Commands;

public class GenerateUserTaskCalendarCommand : ICommand
{
    public Guid Id { get; set; }

    public GenerateUserTaskCalendarCommand(Guid id)
    {
        Id = id;
    }
}

public class GenerateUserTaskCalendarCommandHandler(IUserTaskRepository userTaskRepository, IIcsCalendarGenerator icsCalendarGenerator) : ICommandHandler<GenerateUserTaskCalendarCommand, byte[]>
{
    public async Task<byte[]> Handle(GenerateUserTaskCalendarCommand command, CancellationToken cancellationToken)
    {
        var userTask = await userTaskRepository.GetOneByIdAsync(command.Id, cancellationToken);

        if (userTask is null)
            throw new NotFoundException($"Tarefa com ID {command.Id} n��o encontrada.");

        var icsContent = icsCalendarGenerator.GenerateICalendar(userTask);

        var bytes = Encoding.UTF8.GetBytes(icsContent);
        return bytes;
    }
}