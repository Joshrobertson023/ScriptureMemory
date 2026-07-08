using Microsoft.EntityFrameworkCore;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.EntityConfigurations;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<Bible> Bibles { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Verse> Verses { get; set; }
    // public DbSet<Collection> Collections { get; set; }
    // public DbSet<UserPassage> UserPassages { get; set; }
    public DbSet<VerseTranslationContent> VerseTranslationContents { get; set; }
    public DbSet<SyncLog> BibleSyncLogs { get; set; }
    public DbSet<ExceptionModel> Exceptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AccountConfiguration().Configure(modelBuilder.Entity<Account>());
        new AdminConfiguration().Configure(modelBuilder.Entity<Admin>());
        new BibleConfiguration().Configure(modelBuilder.Entity<Bible>());
        new ChapterConfiguration().Configure(modelBuilder.Entity<Chapter>());
        new SessionConfiguration().Configure(modelBuilder.Entity<Session>());
        new UserConfiguration().Configure(modelBuilder.Entity<User>());
        new UserPreferencesConfiguration().Configure(modelBuilder.Entity<UserPreferences>());
        new VerseConfiguration().Configure(modelBuilder.Entity<Verse>());
        new VerseContentConfiguration().Configure(modelBuilder.Entity<VerseTranslationContent>());
        new SyncLogConfiguration().Configure(modelBuilder.Entity<SyncLog>());
        new ExceptionConfiguration().Configure(modelBuilder.Entity<ExceptionModel>());
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }
}