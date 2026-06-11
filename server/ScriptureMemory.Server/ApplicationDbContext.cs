using Microsoft.EntityFrameworkCore;

namespace ScriptureMemory.Server;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<PaidInfo> PaidInfo { get; set; }
    public DbSet<Bible> Bibles { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Verse> Verses { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<UserPassage> UserPassages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set ids to generate always by default as identity fields
        modelBuilder.Entity<User>()
            .Property(u => u.UserId)
            .UseIdentityByDefaultColumn();
        modelBuilder.Entity<Admin>()
            .Property(u => u.UserId)
            .UseIdentityByDefaultColumn();
        modelBuilder.Entity<Collection>()
            .Property(c => c.Id)
            .UseIdentityByDefaultColumn();
        modelBuilder.Entity<UserPassage>()
            .Property(u => u.Id)
            .UseIdentityByDefaultColumn();
        modelBuilder.Entity<Session>()
            .Property(s => s.Id)
            .UseIdentityByDefaultColumn();
        
        // Conversions from enums to strings
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
        modelBuilder.Entity<Admin>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
        modelBuilder.Entity<UserPreferences>()
            .Property(p => p.ThemePreference)
            .HasConversion<string>()
            .HasMaxLength(20);
        modelBuilder.Entity<UserPreferences>()
            .Property(p => p.BibleVersion)
            .HasConversion<string>()
            .HasMaxLength(20);
        modelBuilder.Entity<UserPreferences>()
            .Property(p => p.CollectionsSort)
            .HasConversion<string>()
            .HasMaxLength(20);
        modelBuilder.Entity<Collection>()
            .Property(c => c.Visibility)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        // Configure owned References
        // modelBuilder.Entity<Passage>()
        //     .OwnsOne(p => p.Reference);
        modelBuilder.Entity<UserPassage>()
            .OwnsOne(p => p.Reference);
        modelBuilder.Entity<Passage>()
            .OwnsOne(p => p.Reference);
        modelBuilder.Entity<Verse>()
            .OwnsOne(p => p.Reference);
        
        // Set default values
        modelBuilder.Entity<Verse>()
            .Property(v => v.MemorizedCount)
            .HasDefaultValue(0);
        modelBuilder.Entity<Verse>()
            .Property(v => v.SavedCount)
            .HasDefaultValue(0);
    }
}