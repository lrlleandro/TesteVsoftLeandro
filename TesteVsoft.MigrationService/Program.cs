using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TesteVsoft.Application.Common.Assemblies;
using TesteVsoft.Infrastructure.Common.Extensions;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var applicationAssembly = typeof(AssemblyReference).Assembly;
var infrastructureAssembly = typeof(TesteVsoft.Infrastructure.Common.Assemblies.AssemblyReference).Assembly;
builder.Services.AddInfrastructureServices(builder.Configuration, applicationAssembly, infrastructureAssembly);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.ApplyMigrationsAsync();
}
