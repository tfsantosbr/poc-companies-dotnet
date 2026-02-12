using Companies.Api.Extensions.Endpoints;
using Companies.Api.Features.Authentication.Requests;
using Companies.Application.Abstractions.Authentication;
using Companies.Application.Abstractions.Results;

namespace Companies.Api.Features.Authentication;

public class AuthenticationEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("authentication").WithTags("Authentication");

        group.MapPost("/token", RequestToken)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static IResult RequestToken(
        CreateTokenRequest request,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        // Ler array de credenciais
        var credentials = configuration.GetSection("TestCredentials")
            .Get<List<TestCredential>>() ?? new List<TestCredential>();

        // Buscar usuário no array
        var user = credentials.FirstOrDefault(c =>
            c.Login == request.Username && c.Password == request.Password);

        if (user == null)
        {
            return Results.NotFound(new Error("User", "User not found"));
        }

        // Gerar token com dados do usuário encontrado
        var token = tokenService.GenerateToken(user.Id, user.Name, user.Email);

        return Results.Ok(new { token });
    }
}
