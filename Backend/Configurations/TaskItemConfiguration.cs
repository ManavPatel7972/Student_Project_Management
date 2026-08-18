using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TaskTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.AssignedScore)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(t => t.EarnedScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(t => t.FacultyRemarks)
                .HasMaxLength(500);

            builder.Property(t => t.StudentRemarks)
                .HasMaxLength(500);

            builder.Property(t => t.TaskAssignedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(t => t.ProjectAllocation)
                .WithMany(pa => pa.Tasks)
                .HasForeignKey(t => t.ProjectAllocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.TaskStatus)
                .WithMany(ts => ts.Tasks)
                .HasForeignKey(t => t.TaskStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TaskPriority)
                .WithMany(tp => tp.Tasks)
                .HasForeignKey(t => t.TaskPriorityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
