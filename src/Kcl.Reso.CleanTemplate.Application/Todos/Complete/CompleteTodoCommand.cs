using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Complete;

public sealed record CompleteTodoCommand(Guid TodoItemId) : ICommand;
