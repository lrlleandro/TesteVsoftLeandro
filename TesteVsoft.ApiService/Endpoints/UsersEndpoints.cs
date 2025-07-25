using Microsoft.AspNetCore.Mvc;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Queries;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.ApiService.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/createRandom", async ([FromBody] CreateRandomUsersCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            await eventDispatcher.SendAsync(command, cancellationToken);
            return Results.Ok(new { Message = "A criação dos usuários começou em segundo plano." });
        })
        .WithName("Create Random Users")
        .WithTags("Users")
        .RequireAuthorization();

        app.MapPost("/users/create", async ([FromBody] CreateUserCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var createdUser = await eventDispatcher.SendAsync<User>(command, cancellationToken);
            return Results.Ok(new { Message = $"Usu\u00e1rio criado com sucesso", User = createdUser });
        })
        .WithName("Create User")
        .WithTags("Users")
        .RequireAuthorization();

        app.MapPost("/users/list", async ([FromBody] ListUsersQuery query, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var users = await eventDispatcher.QueryAsync<PaginatedList<User, Guid>>(query, cancellationToken);
            return Results.Ok(users);
        })
        .WithName("List Users")
        .WithTags("Users")
        .RequireAuthorization();

        app.MapPut("/users/{id}", async (Guid id, [FromBody] UpdateUserCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            command.Id = id;
            await eventDispatcher.SendAsync(command, cancellationToken);
            return Results.NoContent();
        })
        .WithName("Update User")
        .WithTags("Users")
        .RequireAuthorization();

        app.MapDelete("/users/{id}", async (Guid id, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            await eventDispatcher.SendAsync(new DeleteUserCommand(id), cancellationToken);
            return Results.NoContent();
        })
        .WithName("Delete User")
        .WithTags("Users")
        .RequireAuthorization();
    }
}