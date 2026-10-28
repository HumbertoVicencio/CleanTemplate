using Kcl.Reso.CleanTemplate.Domain.Users;

namespace Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);

    string GenerateRefreshToken();
}
