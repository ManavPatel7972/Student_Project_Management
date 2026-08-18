using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class TaskPriorityLookupConfiguration : IEntityTypeConfiguration<TaskPriorityLookup>
    {
        public void Configure(EntityTypeBuilder<TaskPriorityLookup> builder)
        {
            builder.ToTable("TaskPriorities");

            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.TaskPriorityName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(tp => tp.TaskPriorityName)
                .IsUnique();

            builder.Property(tp => tp.TaskPriorityCssClass)
                .HasMaxLength(100);
        }
    }
}
