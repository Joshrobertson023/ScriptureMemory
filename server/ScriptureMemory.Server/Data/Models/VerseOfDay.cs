using System;

namespace DataAccess.Models;

public class VerseOfDay
{
    [Key]
    public string PassageId { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Passage PassageNavigation { get; set; }
}
