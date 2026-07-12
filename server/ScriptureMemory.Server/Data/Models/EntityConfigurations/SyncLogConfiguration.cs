using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class SyncProgressReportConfiguration : IEntityTypeConfiguration<SyncEvent>
{
    public void Configure(EntityTypeBuilder<SyncEvent> builder)
    {
        builder.ToTable("SyncProgressReports");
        builder.Property(b => b.BibleId)
            .HasMaxLength(100);
        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("NOW()");
        builder.HasOne(e => e.Exception)
            .WithOne(ex => ex.SyncLogNavigation)
            .HasForeignKey<ExceptionModel>(ex => ex.SyncReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}