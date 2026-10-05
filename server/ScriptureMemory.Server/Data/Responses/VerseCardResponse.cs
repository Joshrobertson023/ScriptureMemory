using DataAccess.Models;
using System.Text.Json.Serialization;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseCardResponse
{
    public int TotalSaved { get; set; }
    public int TotalMemorized { get; set; }
    public int NumPracticed { get; set; }
    public DateTime NextDue { get; set; }
    public List<CrossReferenceResponse> CrossReferences { get; set; } = new();
    public List<Verse> Similar { get; set; } = new();
    public List<VerseTranslationContent> VerseTexts { get; set; } = new();

    [JsonIgnore]
    public List<Verse> RequestedVerses { get; set; } = new();
}
