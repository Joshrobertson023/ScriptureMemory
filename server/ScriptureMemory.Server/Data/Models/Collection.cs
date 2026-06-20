    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

[NotMapped]
public class Collection
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public User UserNavigation { get; set; } = null!;
    
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
    
    [DefaultValue(CollectionVisibility.Private)]
    public CollectionVisibility Visibility { get; set; }
    
    [DefaultValue("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")]
    public DateTime DateCreated { get; set; }
    
    /// <summary>
    /// Position this collection is ordered on the user's Collections page
    /// </summary>
    public int? OrderPosition { get; set; }
    
    [DefaultValue(false)]
    public bool IsFavorites { get; set; }
    
    [DefaultValue(false)]
    public bool IsUncategorized { get; set; }
    
    [DefaultValue(false)]
    public bool IsArchived { get; set; }
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Memorization progress for the whole collection as a whole number (0-100)
    /// </summary>
    public int? ProgressPercent { get; set; }
    
    public List<UserPassage> Passages { get; set; } = new();
    
    public List<CollectionNote> Notes { get; set; } = new();

    public Collection() { }

    public Collection(
        int userId,
        string title,
        bool isFavorites = false
    )
    {
        UserId = userId;
        Title = title;
        DateCreated = DateTime.UtcNow;
        IsFavorites = isFavorites;
    }
}
