using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class UserTypeConfiguration : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");

            builder.HasKey(ut => ut.Id);

            builder.Property(ut => ut.UserTypeName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(ut => ut.UserTypeName)
                .IsUnique();

            builder.Property(ut => ut.Description)
                .HasMaxLength(250);

         
        }
    }
}
