using Kcl.Reso.CleanTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kcl.Reso.CleanTemplate.Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
