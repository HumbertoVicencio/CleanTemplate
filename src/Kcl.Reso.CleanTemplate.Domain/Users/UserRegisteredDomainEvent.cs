using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
