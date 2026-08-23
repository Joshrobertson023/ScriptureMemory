using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;
using System.Text.Json.Serialization;

namespace DataAccess.Models;

[NotMapped]
public sealed class Reference
{
    private Book _book;

    public Book Book
    {
        get => _book;
        set
        {
            // If chapter has been set before the book, now ensure the chapter is valid for this book
            if (Chapter != 0)
                value.EnsureValidChapter(Chapter);

            _book = value;
        }
    }

    private int _chapter;

    public int Chapter
    {
        get => _chapter;
        set
        {
            if (Book is null)
            {
                _chapter = value;
                return;
            }
            
            Book.EnsureValidChapter(value);
            _chapter = value;
        }
    }
    
    [JsonIgnore]
    public string ChapterId => Book.Abbreviation.ToUpper() 
                   + '.' 
                   + Chapter.ToString();

    [JsonIgnore]
    public string CacheKey => VerseId 
                              ?? throw new InvalidOperationException("Unable to get CacheKey: VerseId was null");


    public string? VerseId
    {
        get
        {
            if (VerseNumbers is null || VerseNumbers.Count <= 0)
                return null;
            
            return Book.Abbreviation.ToUpper() 
                + '.' 
                + Chapter.ToString() 
                + '.'
                + string.Join(',', VerseNumbers.Select(v => v.ToString()));
        }
    }

    public string[] VerseIds
    {
        get
        {
            if (VerseNumbers is null || VerseNumbers.Count <= 0)
                return null;
            
            string[] verseIds = new string[VerseNumbers.Count];

            for (int i = 0; i < VerseNumbers.Count; i++)
            {
                verseIds[i] = Book.Abbreviation.ToUpper() 
                              + '.' 
                              + Chapter.ToString() 
                              + '.'
                              + VerseNumbers[i].ToString();
            }
            
            return verseIds;
        }
    }

    private List<int>? _verseNumbers;

    public List<int>? VerseNumbers
    {
        get => _verseNumbers;
        set
        {
            if (value is null)
                _verseNumbers = null;
            else
                _verseNumbers = new List<int>(value);
        }
    }
    private string _readableReference;

    public string ReadableReference
    {
        get
        {
            if (Book is null || Chapter == 0)
                return "Requested readable reference when Book or Chapter were not initialized.";
            
            if (string.IsNullOrEmpty(_readableReference))
                _readableReference = ReferenceParser.ConvertToReadableReference(Book.DisplayName, Chapter, VerseNumbers);

            return _readableReference;
        }
        set
        {
            if (!string.IsNullOrEmpty(_readableReference))
                return;
            
            _readableReference = value;
        }
    }
    
    /// <summary>
    /// Construct the Reference from a readableReference with every attribute filled out
    /// TODO: Using GetBook(), GetChapter(), and GetIndividualVerses() may be more inefficient than just using Parse().
    /// Plus, putting it through Parse() will ensure ReadableReference has been parsed/valid.
    /// 
    /// </summary>
    /// <param name="readableReference"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Reference(string readableReference)
    {
        Book? book = ReferenceParser.GetBook(readableReference);

        Book = book is not null
            ? book
            : throw new BookNotFoundException($"Not a valid book inside reference.");
        
        Chapter = ReferenceParser.GetChapter(readableReference) 
                  ?? throw new InvalidOperationException($"{readableReference} is not a valid reference.");
        
        VerseNumbers = ReferenceParser.GetIndividualVerses(readableReference);
    }

    public Reference(Book book, int chapter, List<int> verseNumbers)
    {
        Book = book;
        Chapter = chapter;
        VerseNumbers = verseNumbers;
    }

    public Reference(Book book, int chapter, int verseNumber)
    {
        Book = book;
        Chapter = chapter;
        VerseNumbers = [verseNumber];
    }

    public Reference(Book book, int chapter)
    {
        Book = book;
        Chapter = chapter;
    }

    // TODO: Update so readable reference can be made if just a chapter reference
    // Todo: Also make sure it can't change to a verse reference if already a chapter reference
    public Reference(string requestedBook, int chapter)
    {
        Book = new Book(requestedBook);
        Chapter = chapter;
    }

    public Reference(string requestedBook, int chapter, int verseNumber)
    {
        Book = new Book(requestedBook);
        Chapter = chapter;
        VerseNumbers = [verseNumber];
    }
    
    public Reference() { } // For json deserialization

    public override string ToString()
    {
        if (string.IsNullOrEmpty(_readableReference))
            throw new InvalidOperationException(
                "Unable to get Reference ToString, _readableReference is null or empty");
        
        return _readableReference;
    }
}
