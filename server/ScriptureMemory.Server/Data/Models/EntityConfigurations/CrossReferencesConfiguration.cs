using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class CrossReferencesConfiguration : IEntityTypeConfiguration<CrossReference>
{
    public void Configure(EntityTypeBuilder<CrossReference> builder)
    {
        builder.HasOne(d => d.FromVerse)
            .WithMany()
            .HasForeignKey(d => d.FromVerseId);
        builder.HasOne(d => d.ToPassage)
            .WithMany()
            .HasForeignKey(d => d.ToPassageId);
    }
}
