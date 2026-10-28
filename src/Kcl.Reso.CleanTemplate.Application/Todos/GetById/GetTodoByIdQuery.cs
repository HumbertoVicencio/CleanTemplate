using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.GetById;

public sealed record GetTodoByIdQuery(Guid TodoItemId) : IQuery<TodoResponse>;
