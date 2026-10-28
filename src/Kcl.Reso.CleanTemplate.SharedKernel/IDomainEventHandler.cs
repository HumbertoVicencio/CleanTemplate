namespace Kcl.Reso.CleanTemplate.SharedKernel;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
