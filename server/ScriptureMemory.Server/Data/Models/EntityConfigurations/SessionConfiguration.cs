using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(e => e.DeviceId)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.DeviceName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Model)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.RefreshTokenHash)
            .HasMaxLength(100);
        builder.Property(e => e.PushNotificationToken)
            .HasMaxLength(100);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
        builder.Property(e => e.LastSeenAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);
    }
}