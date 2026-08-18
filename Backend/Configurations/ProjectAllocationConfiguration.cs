using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class ProjectAllocationConfiguration : IEntityTypeConfiguration<ProjectAllocation>
    {
        public void Configure(EntityTypeBuilder<ProjectAllocation> builder)
        {
            builder.ToTable("ProjectAllocations");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.AssignedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pa => pa.OverAllGrade)
                .HasMaxLength(10);

            builder.Property(pa => pa.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(pa => pa.Project)
                .WithMany(p => p.Allocations)
                .HasForeignKey(pa => pa.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pa => pa.Student)
                .WithMany(u => u.StudentAllocations)
                .HasForeignKey(pa => pa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.Faculty)
                .WithMany(u => u.FacultyAllocations)
                .HasForeignKey(pa => pa.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one student per project allocation
            builder.HasIndex(pa => new { pa.ProjectId, pa.StudentId })
                .IsUnique();
        }
    }
}
