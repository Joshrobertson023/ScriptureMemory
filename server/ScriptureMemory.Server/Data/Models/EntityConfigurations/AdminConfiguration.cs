using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");
        builder.Property(e => e.AdminEmail)
            .HasMaxLength(50);
        builder.Property(e => e.PersonalEmail)
            .HasMaxLength(50);
    }
}