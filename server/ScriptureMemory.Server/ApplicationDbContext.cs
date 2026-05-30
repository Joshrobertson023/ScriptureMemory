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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}