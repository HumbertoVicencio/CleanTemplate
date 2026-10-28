using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Delete;

public sealed record DeleteTodoCommand(Guid TodoItemId) : ICommand;
