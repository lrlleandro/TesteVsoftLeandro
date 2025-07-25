using MassTransit;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.ApiService.Consumers;

public class UserTaskAssignedToUserConsumer(IEventDispatcher eventDispatcher) : IConsumer<UserTaskAssignedToUserDto>
{
    public async Task Consume(ConsumeContext<UserTaskAssignedToUserDto> context)
    {
    }
}