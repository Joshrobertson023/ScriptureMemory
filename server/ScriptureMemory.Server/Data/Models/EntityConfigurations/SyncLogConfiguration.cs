using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class SyncLogConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("BibleSyncLogs");
        builder.Property(b => b.BibleId)
            .HasMaxLength(100);
        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("NOW()");
        builder.HasOne(e => e.Exception)
            .WithOne(ex => ex.SyncLogNavigation)
            .HasForeignKey<ExceptionModel>(ex => ex.SyncLogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}