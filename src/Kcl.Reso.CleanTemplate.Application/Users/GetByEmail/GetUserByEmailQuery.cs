using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
