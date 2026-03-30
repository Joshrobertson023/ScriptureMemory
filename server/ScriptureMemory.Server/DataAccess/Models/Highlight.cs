using System;

namespace DataAccess.Models;

public class Highlight
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string VerseReference { get; set; }
    public DateTime CreatedDate { get; set; }
}

