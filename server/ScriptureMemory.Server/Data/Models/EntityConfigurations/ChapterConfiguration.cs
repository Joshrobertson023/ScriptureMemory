using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(e => new { e.Id, e.Version });
        builder.OwnsOne(e => e.Book, b =>
        {
            b.Property(p => p.Abbreviation).HasColumnName("Book_Abbreviation");
            b.Ignore(p => p.DisplayName);
            b.Ignore(p => p.FuzzyMatches);
            b.Ignore(p => p.NumChapters);
        });
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(50)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Version)
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.ChapterNum)
            .IsRequired();
        builder.Property(e => e.ContentUsx)
            .IsRequired();
    }
}