namespace ScriptureMemory.Server.Tools.Models;

public class Root
{
    public string Id { get; set; }
    public string BibleId { get; set; }
    public string Number { get; set; }
    public string BookId { get; set; }
    public string Reference { get; set; } // chapter reference
    public string Copyright { get; set; }
    public int VerseCount { get; set; }
    public List<Content> Content { get; set; }
    //public string Content { get; set; }
}