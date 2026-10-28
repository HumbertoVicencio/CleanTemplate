using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Users.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AccessTokensResponse>;
