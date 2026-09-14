namespace ScriptureMemory.Server.Tools.Models;

public class ChapterData<T>
{
    public string Id { get; set; }
    public string BibleId { get; set; }
    public string Reference { get; set; }
    public string Copyright { get; set; }
    public T Content { get; set; }
}