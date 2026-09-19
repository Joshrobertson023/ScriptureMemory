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
using System.Text.Json.Serialization;

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

            if (string.IsNullOrEmpty(Reference.Book.Abbreviation))
            {
                Reference.Book = Books.GetBook(Reference.Book.DisplayName);
            }
            
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

    public string? PassageId { get; set; }

    [JsonIgnore] // Don't cache
    public Passage? PassageNavigation { get; set; } = null!;

    public double? SearchDistance { get; set; }
    
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

    public Verse(Reference reference)
    {
        Reference = reference;
    }

    public void GenerateId(Book book, int chapter, int verseNum)
    {
        this.Id = book.Abbreviation.ToUpper() + '.' + chapter + '.' + verseNum;
    }

    public void GenerateId(string bookAbbreviation, int chapter, int verseNum)
    {
        this.Id = bookAbbreviation.Trim().ToUpper() + '.' + chapter + '.' + verseNum;
    }
}
