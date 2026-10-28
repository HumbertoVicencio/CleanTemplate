using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Domain.Todos;

public sealed record TodoItemCompletedDomainEvent(Guid TodoItemId) : IDomainEvent;
