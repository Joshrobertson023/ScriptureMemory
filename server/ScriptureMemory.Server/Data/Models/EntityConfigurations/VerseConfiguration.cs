using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class VerseConfiguration : IEntityTypeConfiguration<Verse>
{
    public void Configure(EntityTypeBuilder<Verse> builder)
    {
        builder.OwnsOne(p => p.Reference);
        builder.Property(v => v.MemorizedCount)
            .HasDefaultValue(0);
        builder.Property(v => v.SavedCount)
            .HasDefaultValue(0);
        builder.HasOne(v => v.Content)
            .WithOne(v => v.VerseNavigation)
            .HasForeignKey<VerseContent>(v => v.VerseId);
    }
}