using FluentValidation;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Update;

internal sealed class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(c => c.TodoItemId).NotEmpty();

        RuleFor(c => c.Description).NotEmpty().MaximumLength(500);
    }
}
