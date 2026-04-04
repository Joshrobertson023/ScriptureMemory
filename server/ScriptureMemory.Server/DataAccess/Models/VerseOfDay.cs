using System;

namespace DataAccess.Models;

public class VerseOfDay
{
    public int Id { get; set; } = 0;
    public string Reference { get; set; } = string.Empty;
    public List<Verse> Verses { get; set; } = new();
    public int AdminId { get; set; }
    public int OrderPosition { get; set; }
    public DateTime? Date { get; set; }
    public int MostMemorized { get; set; }
    public int MostSaved { get; set; }
}
