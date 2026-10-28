using Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Data;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Update;

internal sealed class UpdateTodoCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    HybridCache cache)
    : ICommandHandler<UpdateTodoCommand>
{
    public async Task<Result> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(
                t => t.Id == command.TodoItemId && t.UserId == userContext.UserId,
                cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        }

        todoItem.Description = command.Description;

        todoItem.Raise(new TodoItemUpdatedDomainEvent(todoItem.Id));

        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(TodoCacheKeys.ById(todoItem.UserId, todoItem.Id), cancellationToken);

        return Result.Success();
    }
}
