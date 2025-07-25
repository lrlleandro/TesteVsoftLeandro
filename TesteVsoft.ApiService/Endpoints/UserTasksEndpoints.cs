using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Queries;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.ApiService.Endpoints;

public static class UserTasksEndpoints
{
    public static void MapUserTasksEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/tasks/create", async ([FromBody] CreateUserTaskCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            command.UserId = Guid.Parse(userId);

            var task = await eventDispatcher.SendAsync<UserTask>(command, cancellationToken);
            return Results.Ok(task);
        })
        .WithName("Create Task")
        .Produces<UserTask>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        app.MapPost("/tasks/user/assign", async ([FromBody] AssignUserToUserTaskCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            command.UserId = Guid.Parse(userId);

            var task = await eventDispatcher.SendAsync<UserTask>(command, cancellationToken);
            return Results.Ok(task);
        })
        .WithName("Assign User to Task")
        .Produces<UserTask>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        app.MapPost("/tasks/status/change", async ([FromBody] ChangeUserTaskStatusCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            command.UserId = Guid.Parse(userId);

            var task = await eventDispatcher.SendAsync<UserTask>(command, cancellationToken);
            return Results.Ok(task);
        })
        .WithName("Change Task Status")
        .Produces<UserTask>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        //app.MapGet("/tasks/{id}", async ([FromRoute] Guid id, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        //{
        //    var user = httpContext.User;

        //    if (user?.Identity?.IsAuthenticated != true)
        //        return Results.Unauthorized();

        //    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        //    if (string.IsNullOrEmpty(userId))
        //        return Results.Unauthorized();

        //    var query = new GetUserTaskByIdQuery { Id = id, UserId = Guid.Parse(userId) };

        //    var task = await eventDispatcher.QueryAsync<UserTask>(query, cancellationToken);
        //    return Results.Ok(task);
        //})
        //.WithName("Get Task By Id")
        //.Produces<UserTask>(StatusCodes.Status200OK)
        //.Produces(StatusCodes.Status401Unauthorized)
        //.WithTags("Tasks")
        //.RequireAuthorization();

        app.MapPost("/tasks/list", async ([FromBody] ListUserTasksQuery query, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var tasksResult = await eventDispatcher.QueryAsync<PaginatedList<UserTask, Guid>>(query, cancellationToken);

            return Results.Ok(tasksResult);
        })
        .WithName("List Tasks")
        .Produces<PaginatedList<UserTask, Guid>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        app.MapPut("/tasks/update", async ([FromBody] UpdateUserTaskCommand command, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            command.UserId = Guid.Parse(userId);

            await eventDispatcher.SendAsync<UserTask>(command, cancellationToken);
            return Results.NoContent();
        })
        .WithName("Update Task")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        app.MapDelete("/tasks/{id}", async ([FromRoute] Guid id, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
                return Results.Unauthorized();

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var command = new DeleteUserTaskCommand(id);

            await eventDispatcher.SendAsync(command, cancellationToken);
            return Results.NoContent();
        })
        .WithName("Delete Task")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("Tasks")
        .RequireAuthorization();

        app.MapGet("/task/calendar/{id}", async ([FromRoute] Guid id, [FromServices] IEventDispatcher eventDispatcher, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var command = new GenerateUserTaskCalendarCommand(id);

            var bytes = await eventDispatcher.SendAsync<byte[]>(command, cancellationToken);
            return Results.File(bytes, "text/calendar", "evento.ics");
        })
        .WithName("Get Task Calendar")
        .WithTags("Tasks");
    }
}