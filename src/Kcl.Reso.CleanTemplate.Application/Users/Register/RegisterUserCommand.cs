using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
