using Microsoft.EntityFrameworkCore;

namespace ScriptureMemory.Server;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

    protected override void OnMOdelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Verse>()
            .ToTable("Verses")
            .HasKey(v => v.Id);
    }
}