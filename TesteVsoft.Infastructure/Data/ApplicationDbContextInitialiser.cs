using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Data;

[Scoped]
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    
    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task ApplyMigrationsAsync()
    {
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await _context.Database.MigrateAsync();
        }
    }    
}