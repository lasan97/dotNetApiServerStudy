using apiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace apiTest.Common.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(user => user.Id);

        entity.HasIndex(user => user.Email)
            .IsUnique();

        entity.Property(user => user.Email)
            .HasMaxLength(User.EmailMaxLength)
            .IsRequired();

        entity.Property(user => user.Name)
            .HasMaxLength(User.NameMaxLength)
            .IsRequired();

        entity.Property(user => user.PasswordHash)
            .HasMaxLength(User.PasswordHashMaxLength)
            .IsRequired();
    }
}