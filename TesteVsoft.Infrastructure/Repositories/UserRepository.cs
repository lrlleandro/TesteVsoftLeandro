using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Infrastructure.Repositories.Common;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Repositories;

[Scoped]
public sealed class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}
