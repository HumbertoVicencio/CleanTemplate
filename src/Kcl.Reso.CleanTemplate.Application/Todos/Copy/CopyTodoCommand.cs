using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Todos.Copy;

public sealed class CopyTodoCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }
    public Guid TodoId { get; set; }
}
