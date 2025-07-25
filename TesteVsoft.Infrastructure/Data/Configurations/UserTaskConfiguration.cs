using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Infrastructure.Data.Configurations;

public class UserTaskConfiguration : IEntityTypeConfiguration<UserTask>
{
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(u => u.AssignedUserId)
            .HasColumnName("assigned_user_id");
            
        builder.HasOne(u => u.AssignedUser)
            .WithMany()
            .HasForeignKey(u => u.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(u => u.Description)
            .HasColumnName("description");

        builder.Property(u => u.DueDate)
            .HasColumnName("due_date");

        builder.Property(u => u.Status)
            .HasColumnName("task_status")
            .IsRequired();

        builder.Property(u => u.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");
    }
}