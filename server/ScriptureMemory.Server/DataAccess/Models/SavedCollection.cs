using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class SavedCollection : Collection
{
    public int PublishedId { get; set; }
    public int AuthorId { get; set; }
    public string Author { get; set; } = string.Empty;
}
