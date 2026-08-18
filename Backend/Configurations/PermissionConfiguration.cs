using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PermissionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.PermissionName)
                .IsUnique();

            builder.Property(p => p.Description)
                .HasMaxLength(250);
        }
    }
}
