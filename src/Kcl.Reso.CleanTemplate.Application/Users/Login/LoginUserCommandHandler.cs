using Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Data;
using Kcl.Reso.CleanTemplate.Application.Abstractions.Messaging;
using Kcl.Reso.CleanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<LoginUserCommand, AccessTokensResponse>
{
    public async Task<Result<AccessTokensResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.NotFoundByEmail);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.NotFoundByEmail);
        }

        string accessToken = tokenProvider.Create(user);
        string refreshToken = tokenProvider.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresOnUtc = dateTimeProvider.UtcNow.AddDays(RefreshTokenExpirationInDays)
        };

        context.RefreshTokens.Add(refreshTokenEntity);

        await context.SaveChangesAsync(cancellationToken);

        return new AccessTokensResponse(accessToken, refreshToken);
    }

    private const int RefreshTokenExpirationInDays = 7;
}
