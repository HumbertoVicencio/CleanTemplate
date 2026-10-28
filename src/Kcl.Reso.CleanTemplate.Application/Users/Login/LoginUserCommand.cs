using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<AccessTokensResponse>;
