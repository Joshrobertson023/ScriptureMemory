using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class UserConfiguration : AccountConfiguration, IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(e => e.Username)
            .HasMaxLength(20);
        builder.Property(e => e.FirstName)
            .HasMaxLength(20);
        builder.Property(e => e.LastName)
            .HasMaxLength(20);
        builder.Property(e => e.Email)
            .HasMaxLength(30);
        builder.Property(e => e.ProfileDescription)
            .HasMaxLength(100);
        builder.Property(e => e.ProfilePictureUrl)
            .HasMaxLength(100);
        builder.Property(e => e.VersesMemorizedCount)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(e => e.Points)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(e => e.CollectionsCount)
            .IsRequired()
            .HasDefaultValue(0);
        builder.HasOne(e => e.Preferences)
            .WithOne(e => e.UserNavigation)
            .HasForeignKey<UserPreferences>(e => e.UserId);
        builder.HasMany(e => e.Collections)
            .WithOne(e => e.UserNavigation)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}