using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Common.Attributes;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Infrastructure.Repositories.Common;

namespace TesteVsoft.Infrastructure.Repositories;

[Scoped]
public sealed class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}
