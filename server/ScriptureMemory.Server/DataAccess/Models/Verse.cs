using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgvector;
using ScriptureMemory.Server.Tools;
using static DataAccess.Data.VerseData;

namespace DataAccess.Models;


public class Verse
{
    public Reference Reference { get; set; } = new();
    
    /// <summary>
    /// ("PSA.1.1")
    /// </summary>
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;
    
    [MaxLength(30)]
    public string? Book { get; set; }
    public int Chapter { get; set; }
    public int VerseNum { get; set; }
    public int MemorizedCount { get; set; }
    public int SavedCount { get; set; }
    
    public Verse(string id, )

    public string GetBook()
    {
        string.IsNullOrEmpty(Book)
            ? Books.TryGetBook()
    }
}
