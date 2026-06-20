using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(e => e.UserId);
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.HashedPassword)
            .HasMaxLength(100);
        builder.HasMany(e => e.Sessions)
            .WithOne(e => e.AccountNavigation)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Property(e => e.DateCreated)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}