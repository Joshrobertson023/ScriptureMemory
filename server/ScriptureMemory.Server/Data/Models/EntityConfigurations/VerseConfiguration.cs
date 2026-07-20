using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class VerseConfiguration : IEntityTypeConfiguration<Verse>
{
    public void Configure(EntityTypeBuilder<Verse> builder)
    {
        builder.OwnsOne(p => p.Reference, r =>
        {
            r.Property(x => x.ReadableReference).HasColumnName("Reference_ReadableReference");
            r.Property(x => x.Chapter).HasColumnName("Reference_Chapter");
            r.Property(x => x.VerseNumbers).HasColumnName("Reference_VerseNumbers");
            r.OwnsOne(x => x.Book, b =>
            {
                b.Property(t => t.DisplayName).HasColumnName("Reference_Book_DisplayName");
                b.Ignore(t => t.Abbreviation);
                b.Ignore(t => t.FuzzyMatches);
                b.Ignore(t => t.NumChapters);
            });
        });
        builder.Property(v => v.MemorizedCount)
            .HasDefaultValue(0);
        builder.Property(v => v.SavedCount)
            .HasDefaultValue(0);
        builder.HasMany(v => v.TranslationContents)
            .WithOne(v => v.VerseNavigation)
            .HasForeignKey(v => v.VerseId);
    }
}