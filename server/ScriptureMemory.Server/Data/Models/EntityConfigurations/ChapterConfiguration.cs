using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(e => new { e.Id, e.Version });
        // Id convention: a standard readable reference ("Psalms 1")
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(50)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Version)
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.Book)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ChapterNum)
            .IsRequired();
        builder.Property(e => e.ContentUsx)
            .IsRequired();
    }
}