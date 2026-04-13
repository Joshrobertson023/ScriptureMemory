    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;
public class Collection
{
    public int Id { get; set; }
    public int UserId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public CollectionVisibility Visibility { get; set; } = CollectionVisibility.Private;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public int? OrderPosition { get; set; }
    public bool IsFavorites { get; set; } = false;
    public bool IsUncategorized { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    public string? Description { get; set; }
    public int? ProgressPercent { get; set; } // Percentage out of 100
    //public float? AverageProgressPercent { get; set; }
    public int NumPassages { get; set; }
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
