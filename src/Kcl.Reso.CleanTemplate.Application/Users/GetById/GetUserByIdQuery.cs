using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;

namespace Kcl.Reso.CleanTemplate.Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
