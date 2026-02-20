using System;

namespace DataAccess.Models;

public class VerseNote
{
    public int Id { get; set; }
    public string VerseReference { get; set; }
    public string Username { get; set; }
    public string Text { get; set; }
    public bool IsPublic { get; set; }
    public bool? Approved { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? OriginalReference { get; set; }
    public int LikeCount { get; set; }
    public bool UserLiked { get; set; }
}

