using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class TaskStatusLookupConfiguration : IEntityTypeConfiguration<TaskStatusLookup>
    {
        public void Configure(EntityTypeBuilder<TaskStatusLookup> builder)
        {
            builder.ToTable("TaskStatuses");

            builder.HasKey(ts => ts.Id);

            builder.Property(ts => ts.TaskStatusName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(ts => ts.TaskStatusName)
                .IsUnique();

            builder.Property(ts => ts.TaskStatusCssClass)
                .HasMaxLength(100);
        }
    }
}
