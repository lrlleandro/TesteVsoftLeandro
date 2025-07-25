using Microsoft.AspNetCore.Mvc;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.ApiService.Endpoints;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async ([FromBody] LoginCommand command, [FromServices] IEventDispatcher eventDispatcher, CancellationToken cancellationToken) =>
        {
            var token = await eventDispatcher.SendAsync<JwtTokenDto>(command, cancellationToken);
            return Results.Ok(token);
        })
        .WithName("Login")
        .WithTags("Auth");
    }

    
}

