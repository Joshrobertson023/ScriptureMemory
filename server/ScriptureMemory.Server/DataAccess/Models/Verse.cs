using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgvector;
using ScriptureMemory.Server.Tools;
using System.ComponentModel;
using static DataAccess.Data.VerseData;

namespace DataAccess.Models;


public class Verse
{
    public Reference Reference { get; set; }
    
    /// <summary>
    /// ("PSA.1.1")
    /// </summary>
    [Key]
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;
    
    [DefaultValue(0)]
    public int MemorizedCount { get; set; }
    
    [DefaultValue(0)]
    public int SavedCount { get; set; }

    public Passage? PassageNavigation { get; set; } = null!;
    
    public VerseContent? VerseContent { get; set; }
    
    public Verse()

    public string GetBook()
    {
        string.IsNullOrEmpty(Book)
            ? Books.TryGetBook()
    }
}
