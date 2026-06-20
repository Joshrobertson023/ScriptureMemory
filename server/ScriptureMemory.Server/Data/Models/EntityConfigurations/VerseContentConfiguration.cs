using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class VerseContentConfiguration : IEntityTypeConfiguration<VerseTranslationContent>
{
    public void Configure(EntityTypeBuilder<VerseTranslationContent> builder)
    {
        builder.HasKey(c => new { c.Version, c.VerseId });
        builder.Property(e => e.Version)
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.PlainText)
            .IsRequired()
            .HasMaxLength(200); // Longest verse in the Bible, Esther 8:9, contains 90 words
        builder.Property(e => e.LastUpdated);
    }
}