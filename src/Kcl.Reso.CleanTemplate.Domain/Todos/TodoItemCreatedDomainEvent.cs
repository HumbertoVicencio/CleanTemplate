using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Domain.Todos;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
