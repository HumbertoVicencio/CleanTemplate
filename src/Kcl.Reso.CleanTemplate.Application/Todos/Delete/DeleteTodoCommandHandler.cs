using Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Data;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Delete;

internal sealed class DeleteTodoCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    HybridCache cache)
    : ICommandHandler<DeleteTodoCommand>
{
    public async Task<Result> Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoItemId && t.UserId == userContext.UserId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        context.TodoItems.Remove(todoItem);

        todoItem.Raise(new TodoItemDeletedDomainEvent(todoItem.Id));

        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(TodoCacheKeys.ById(todoItem.UserId, todoItem.Id), cancellationToken);

        return Result.Success();
    }
}
