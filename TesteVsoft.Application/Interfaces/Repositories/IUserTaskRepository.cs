using TesteVsoft.Application.Interfaces.Repositories.Common;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Interfaces.Repositories;

public interface IUserTaskRepository : IRepository<UserTask, Guid>
{
}
