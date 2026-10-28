using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Users.Register;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            Request request,
            ICommandHandler<RegisterUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireRateLimiting(RateLimitingPolicies.Authentication);
    }
}
