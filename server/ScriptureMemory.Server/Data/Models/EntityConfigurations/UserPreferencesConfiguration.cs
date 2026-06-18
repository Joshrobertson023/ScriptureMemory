using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.ToTable("UserPreferences");
        builder.Property(e => e.ThemePreference)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValueSql("'SystemDefault'");
        builder.Property(p => p.BibleVersion)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValueSql("'Kjv'");
        builder.Property(p => p.CollectionsSort)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValueSql("'Newest'");
        builder.Property(u => u.ThemePreference)
            .HasDefaultValue(ThemePreference.SystemDefault);
        builder.Property(u => u.BibleVersion)
            .HasDefaultValue(BibleVersion.Kjv);
        builder.Property(u => u.CollectionsSort)
            .HasDefaultValue(CollectionsSort.Newest);
        builder.Property(u => u.SubscribedVerseOfDay)
            .HasDefaultValue(true);
        builder.Property(u => u.NotifyFriendsMemorizedPassage)
            .HasDefaultValue(true);
        builder.Property(u => u.NotifyFriendsPublishedCollection)
            .HasDefaultValue(true);
        builder.Property(u => u.NotifyCollectionSaved)
            .HasDefaultValue(true);
        builder.Property(u => u.NotifyNoteLikedCommented)
            .HasDefaultValue(true);
        builder.Property(u => u.FriendsActivityNotificationsEnabled)
            .HasDefaultValue(true);
        builder.Property(u => u.OverdueRemindersEnabled)
            .HasDefaultValue(true);
        builder.Property(u => u.TypeOutReference)
            .HasDefaultValue(false);
    }
}