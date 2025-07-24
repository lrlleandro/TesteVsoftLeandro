using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteVsoft.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net.Mail;

namespace TesteVsoft.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasColumnName("username")
            .IsRequired();

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .IsRequired();

        builder.Property(u => u.LastLoginOn)
            .HasColumnName("last_login_on");

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasConversion(new ValueConverter<MailAddress, string>(
                v => v == null ? null : v.Address,
                v => v == null ? null : new MailAddress(v)));
    }
}