using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(e => new { e.Id, e.Version });
        // Id convention: a standard readable reference ("Psalms 1")
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