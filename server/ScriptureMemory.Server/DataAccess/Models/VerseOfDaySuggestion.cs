namespace DataAccess.Models;

public class VerseOfDaySuggestion
{
    public int Id { get; set; }
    public string ReadableReference { get; set; } = string.Empty;
    public string SuggesterUsername { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED
}



