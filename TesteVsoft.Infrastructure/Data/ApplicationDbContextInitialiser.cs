using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TesteVsoft.Domain.Entities;
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

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_context.Set<User>().Any(u => u.UserName == "admin"))
        {
            var administrator = User.Create("Administrator", "admin", "vsoft", "lrlleandro@gmail.com");
            _context.Set<User>().Add(administrator);
            await _context.SaveChangesAsync();
        }
    }
}