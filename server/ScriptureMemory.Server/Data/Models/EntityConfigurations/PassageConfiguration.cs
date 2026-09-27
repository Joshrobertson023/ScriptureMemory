using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class PassageConfiguration : IEntityTypeConfiguration<Passage>
{
    public void Configure(EntityTypeBuilder<Passage> builder)
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
        builder.HasMany(b => b.Verses)
            .WithMany(v => v.Passages);
    }
}
