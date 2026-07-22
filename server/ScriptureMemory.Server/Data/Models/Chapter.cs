using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Chapter
{
    public Reference Reference { get; set; }

    private string _id;

    public string Id
    {
        get
        {
            if (Reference.Chapter == 0)
                throw new InvalidOperationException("Requested Id when Chapter was not initialized.");
            
            if (string.IsNullOrEmpty(_id))
            {
                _id = Reference.ChapterId;
            }

            return _id;
        }
        private set
        {
            _id = value;
        }
    }
    
    public string Version { get; set; } = string.Empty;
    
    [Column(TypeName = "text")]
    public string ContentUsx { get; set; } = string.Empty; // Unified Scripture XML
    
    public DateTime? LastUpdated { get; set; }
    
    public Chapter() { }

    public Chapter(string book, int chapter)
    {
        Reference = new Reference(book, chapter);
        
    }
}