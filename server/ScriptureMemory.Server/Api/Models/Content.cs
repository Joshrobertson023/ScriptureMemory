namespace ScriptureMemory.Server.Tools.Models;

public class Content
{
    public string Name { get; set; } // "para", "verse", "char"
    public string Text { get; set; } 
    public string Type { get; set; } // "tag", "text"
    public Dictionary<string, object> Attrs { get; set; } // style, verseId, sid, closed, verseOrgIds[]
    public List<Content> Items { get; set; }
}