using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.Complete;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class Complete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id:guid}/complete", async (
            Guid id,
            ICommandHandler<CompleteTodoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CompleteTodoCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
