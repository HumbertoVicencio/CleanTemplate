using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Update;

public sealed record UpdateTodoCommand(
    Guid TodoItemId,
    string Description) : ICommand;
