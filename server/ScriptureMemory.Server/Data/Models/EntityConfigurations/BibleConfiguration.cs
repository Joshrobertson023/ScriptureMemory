using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class BibleConfiguration : IEntityTypeConfiguration<Bible>
{
    public void Configure(EntityTypeBuilder<Bible> builder)
    {
        builder.HasKey(e => e.Version);
        builder.Property(e => e.Id)
            .HasMaxLength(100);
        builder.Property(e => e.Version)
            .ValueGeneratedOnAdd()
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.VersionFull)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Copyright)
            .HasMaxLength(300);
        builder.Property(e => e.Source)
            .IsRequired()
            .HasMaxLength(300);
    }
}