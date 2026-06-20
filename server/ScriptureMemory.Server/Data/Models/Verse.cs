using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgvector;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;
using System.ComponentModel;
//using static DataAccess.Data.VerseData;

namespace DataAccess.Models;


public class Verse
{
    public Reference Reference { get; set; } = new();
    
    /// <summary>
    /// ("PSA.1.1")
    /// </summary>
    [Key]
    [MaxLength(20)]
    public string Id { get; set; } = string.Empty;

    public int MemorizedCount { get; set; } = 0;

    public int SavedCount { get; set; } = 0;

    public int PassageId { get; set; }
    
    public Passage? PassageNavigation { get; set; } = null!;
    
    public List<VerseTranslationContent>? Translations { get; set; }

    /// <summary>
    /// Creates a new verse, giving it a new VerseId and also ensures a valid parsed reference
    /// </summary>
    public Verse(Book book, int chapter, int verseNum)
    {
        Reference = ReferenceParser.Parse(book.DisplayName, chapter, new List<int>() {verseNum});
        Id = CreateId();
    }
    
    public Verse() { }

    /// <summary>
    /// Creates a new verse, giving it a new VerseId and also ensures it's a valid parsed reference
    /// </summary>
    /// <param name="readableReference"></param>
    /// <exception cref="ArgumentException"></exception>
    public Verse(string readableReference)
    {
        Reference = ReferenceParser.Parse(readableReference)
            ?? throw new ArgumentException($"{readableReference} is not a valid reference");
        Id = CreateId();
        
    }

    /// <summary>
    /// Creates an Id for this verse (example: "PSA.1.1")
    /// </summary>
    /// <returns></returns>
    public string CreateId()
    {
        return Reference.Book.Abbreviation
               + '.'
               + Reference.Chapter
               + '.'
               + Reference.VerseNumbers.First();
    }
}
