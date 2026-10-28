using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId) : IQuery<List<TodoResponse>>;
