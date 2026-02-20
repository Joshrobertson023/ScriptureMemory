using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public class PublishedCollection
{
    public int PublishedId { get; set; }
    public string? Description { get; set; }
    public int NumSaves { get; set; }
    public string Title { get; set; }
    public string VerseOrder { get; set; }
    public string Author { get; set; }
    public int? CollectionId { get; set; }
    public List<int> categoryIds { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Status { get; set; } = "PENDING";
    public List<UserPassage> UserVerses { get; set; } = new();
    public List<CollectionNote> Notes { get; set; } = new();
}



