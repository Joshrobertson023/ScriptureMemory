    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;
public class Collection
{
    [Key]
    public int Id { get; set; }

    [InverseProperty(nameof(User.UserId))]
    public User User { get; set; } = new();
    
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
    
    [DefaultValue(CollectionVisibility.Private)]
    public CollectionVisibility Visibility { get; set; }
    
    [DefaultValue("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")]
    public DateTime DateCreated { get; set; }
    
    public int? OrderPosition { get; set; } // Position this collection is ordered on the user's Collections page
    
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
    
    [InverseProperty(nameof(CollectionNote.CollectionNavigation))]
    public List<UserPassage> Passages { get; set; } = new();
    
    [InverseProperty(nameof(CollectionNote.CollectionNavigation))]
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
