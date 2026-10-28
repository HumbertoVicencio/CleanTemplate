using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.GetById;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/{id:guid}", async (
            Guid id,
            IQueryHandler<GetTodoByIdQuery, TodoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GetTodoByIdQuery(id);

            Result<TodoResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
