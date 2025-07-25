using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TesteVsoft.ApiService.Consumers;
using TesteVsoft.ApiService.Endpoints;
using TesteVsoft.ApiService.Extensions;
using TesteVsoft.ApiService.Workers;
using TesteVsoft.Infrastructure.Common.Extensions;
using TesteVsoft.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddRabbitMQClient(connectionName: "messaging");

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

var applicationAssembly = typeof(TesteVsoft.Application.Common.Assemblies.AssemblyReference).Assembly;
var infrastructureAssembly = typeof(TesteVsoft.Infrastructure.Common.Assemblies.AssemblyReference).Assembly;

builder.Services.AddInfrastructureServices(builder.Configuration,
    applicationAssembly,
    infrastructureAssembly);


builder.Services.AddCors(c =>
{
    c.AddPolicy("allowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserTaskAssignedToUserConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var configService = ctx.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("messaging");
        Console.WriteLine(connectionString);
        cfg.Host(connectionString);

        cfg.ReceiveEndpoint(builder.Configuration["SendEmailQueue"]!, e =>
        {
            e.ConfigureConsumer<UserTaskAssignedToUserConsumer>(ctx);
        });
    });

});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Teste Vsoft API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type=Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHostedService<UserCreationWorker>();

var app = builder.Build();

app.UseCors("allowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandlingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapLoginEndpoints();

app.Run();