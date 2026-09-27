using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class VerseofDayConfiguration : IEntityTypeConfiguration<VerseOfDay>
{
    public void Configure(EntityTypeBuilder<VerseOfDay> builder)
    {
        builder.ToTable("VerseOfDays");
        builder.Property(b => b.Date).HasDefaultValueSql("CURRENT_DATE");
    }
}
