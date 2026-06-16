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
    }
}