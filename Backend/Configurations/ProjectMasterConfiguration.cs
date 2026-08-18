using Backend.Enums;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class ProjectMasterConfiguration : IEntityTypeConfiguration<ProjectMaster>
    {
        public void Configure(EntityTypeBuilder<ProjectMaster> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProjectTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description);

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .HasDefaultValue(ProjectStatus.NotStarted);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
