using System;
using System.Collections.Generic;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Models;

public class PublishedCollection
{
    public int PublishedId { get; set; }
    public string? Description { get; set; }
    public int OrderPosition { get; set; }
    public int NumSaves { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public int? CollectionId { get; set; }
    public List<int> categoryIds { get; set; } = new();
    public DateTime DatePublished { get; set; } = DateTime.UtcNow;
    public ApprovedStatus Status { get; set; }
    public List<UserPassage> UserVerses { get; set; } = new();
    public List<CollectionNote> Notes { get; set; } = new();
}



