using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.PasswordHash)
            .IsRequired();
            
        builder.Property(u => u.PasswordSalt)
            .IsRequired();
            
        // Add unique index on email
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}