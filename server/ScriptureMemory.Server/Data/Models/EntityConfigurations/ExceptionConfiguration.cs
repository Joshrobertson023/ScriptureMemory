using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class ExceptionConfiguration : IEntityTypeConfiguration<ExceptionModel>
{
    public void Configure(EntityTypeBuilder<ExceptionModel> builder)
    {
        builder.ToTable("Exceptions");
        builder.Property(e => e.Message)
            .HasMaxLength(200);
        builder.Property(e => e.Source)
            .HasMaxLength(30);
        builder.Property(e => e.StackTrace)
            .HasMaxLength(300);
        builder.Property(e => e.Type)
            .HasMaxLength(30);
        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("NOW()");
    }
}