using Kcl.Reso.CleanTemplate.Domain.Users;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}

internal sealed class UserRegisteredDomainEventHandler1 : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
