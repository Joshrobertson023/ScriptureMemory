namespace ScriptureMemory.Server.Data.Requests.Post;

public class GetSimilarRequest
{
    [Required] public Reference Reference { get; set; }
    [Required] public string Translation { get; set; }
    public double? LastVerseDistance { get; set; } = null;
}
