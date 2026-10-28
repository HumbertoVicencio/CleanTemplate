# Endpoint Templates

One file per use case in `src/Kcl.Reso.CleanTemplate.Web.Api/Endpoints/{Feature}/{UseCase}.cs`. Endpoints implement `IEndpoint` and are auto-discovered by `AddEndpoints`/`MapEndpoints` — no registration needed.

## Command with response body (POST → 200 + value)

```csharp
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.Create;
using Kcl.Reso.CleanTemplate.Domain.Todos;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = [];
        public int Priority { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos", async (
            Request request,
            ICommandHandler<CreateTodoCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTodoCommand
            {
                UserId = request.UserId,
                Description = request.Description,
                DueDate = request.DueDate,
                Labels = request.Labels,
                Priority = (Priority)request.Priority
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
```

## Void command from route parameter (PUT/DELETE → 204)

```csharp
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.Archive;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class Archive : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id:guid}/archive", async (
            Guid id,
            ICommandHandler<ArchiveTodoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ArchiveTodoCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
```

## Query (GET → 200)

```csharp
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Application.Todos.GetOverdue;
using Kcl.Reso.CleanTemplate.SharedKernel;
using Kcl.Reso.CleanTemplate.Web.Api.Extensions;
using Kcl.Reso.CleanTemplate.Web.Api.Infrastructure;

namespace Kcl.Reso.CleanTemplate.Web.Api.Endpoints.Todos;

internal sealed class GetOverdue : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/overdue", async (
            IQueryHandler<GetOverdueTodosQuery, List<TodoResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOverdueTodosQuery();

            Result<List<TodoResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
```

## Rules

- Routes are lowercase, plural, no leading slash: `todos`, `todos/{id:guid}`, `users/{userId:guid}/todos`. Route constraints (`:guid`) on all typed parameters.
- The nested `Request` class exists only when there's a JSON body; it maps 1:1 to the command inside the lambda (enum values arrive as `int` and are cast).
- Resolve the handler interface (`ICommandHandler<...>` / `IQueryHandler<...>`) directly as a lambda parameter — the decorated instance is injected.
- Always end with `.WithTags(Tags.{Feature})` and `.RequireAuthorization()` (or `.HasPermission(Permissions.X)` where a permission constant exists). Add the feature constant to `Tags.cs` if new.
- Failures never get hand-rolled responses — `CustomResults.Problem` translates the `Error` to RFC 7807 ProblemDetails with the right status code.
