using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class BibleConfiguration : IEntityTypeConfiguration<Bible>
{
    public void Configure(EntityTypeBuilder<Bible> builder)
    {
        builder.HasKey(e => e.Version);
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Version)
            .ValueGeneratedOnAdd()
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Copyright)
            .HasMaxLength(700);
        builder.Property(e => e.Info)
            .IsRequired()
            .HasMaxLength(800);
    }
}