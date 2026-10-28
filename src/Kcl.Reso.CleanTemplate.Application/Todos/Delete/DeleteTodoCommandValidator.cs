using FluentValidation;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Delete;

internal sealed class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(c => c.TodoItemId).NotEmpty();
    }
}
