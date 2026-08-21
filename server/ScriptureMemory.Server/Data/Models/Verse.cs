using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgvector;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;
using System.ComponentModel;
using System.Runtime.Serialization;

//using static DataAccess.Data.VerseData;

namespace DataAccess.Models;


public class Verse
{
    private Reference _reference;
    
    public Reference Reference 
    { 
        get => _reference;
        set
        {
            _reference = value;
            
            this.Id = Reference.Book.Abbreviation.ToUpper()
                      + '.'
                      + Reference.Chapter
                      + '.'
                      + Reference.VerseNumbers.First();
        }
    }
    
    /// <summary>
    /// ("PSA.1.1")
    /// </summary>
    [Key]
    [MaxLength(20)]
    public string Id { get; set; }

    public int MemorizedCount { get; set; } = 0;

    public int SavedCount { get; set; } = 0;

    public int? PassageId { get; set; }
    
    public Passage? PassageNavigation { get; set; } = null!;

    public string CacheKey => Reference.CacheKey;
    
    public List<VerseTranslationContent>? TranslationContents { get; set; }

    public Verse(Book book, int chapter, int verseNum)
    {
        Reference = new Reference(book, chapter, verseNum);
    }

    public Verse(string bookName, int chapter, int verseNum)
    {
        Reference = new Reference(bookName, chapter, verseNum);
    }
    
    public Verse() { }

    public Verse(string readableReference)
    {
        Reference = new Reference(readableReference);
    }

    [OnSerializing]
    internal void ConvertVectorToArrays()
    {
        foreach (var content in TranslationContents)
        {
            if (!content.JsonEmbeddings.Any())
            {
                content.JsonEmbeddings = content.Embedding?.ToArray()
                    ?? throw new ArgumentNullException(nameof(content.Embedding));
            }
        }
    }

    [OnDeserialized]
    internal void ConvertArrayEmbeddingsToVector()
    {
        foreach (var content in TranslationContents)
        {
            if (content.JsonEmbeddings.Any())
            {
                content.Embedding = new Vector(content.JsonEmbeddings);
            }
        }
    }
}
