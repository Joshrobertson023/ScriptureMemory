using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScriptureMemory.Server.Data.Models.EntityConfigurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(e => new { e.Id, e.Version });
        builder.OwnsOne(e => e.Reference, b =>
        {
            b.Property(p => p.Chapter).HasColumnName("Reference_Chapter");
            b.OwnsOne(p => p.Book, r =>
            {
                r.Property(prop => prop.DisplayName).HasColumnName("Reference_Book_DisplayName");
            });
        });
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(50)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Version)
            .IsRequired()
            .HasMaxLength(5);
        builder.Property(e => e.ContentUsx)
            .IsRequired();
    }
}