using Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Data;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Domain.Todos;
using Kcl.Reso.CleanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Create;

internal sealed class CreateTodoCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext)
    : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Labels = command.Labels,
            IsCompleted = false,
            CreatedAt = dateTimeProvider.UtcNow
        };

        todoItem.Raise(new TodoItemCreatedDomainEvent(todoItem.Id));

        context.TodoItems.Add(todoItem);

        await context.SaveChangesAsync(cancellationToken);

        return todoItem.Id;
    }
}
