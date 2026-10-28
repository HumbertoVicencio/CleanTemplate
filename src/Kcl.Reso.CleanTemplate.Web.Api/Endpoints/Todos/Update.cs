using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.Update;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class Update : IEndpoint
{
    public sealed record Request(string Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateTodoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTodoCommand(id, request.Description);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
