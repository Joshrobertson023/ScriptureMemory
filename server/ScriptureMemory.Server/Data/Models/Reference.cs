using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;

namespace DataAccess.Models;

[NotMapped]
public sealed class Reference
{
    private string _readableReference;

    public string ReadableReference
    {
        get => _readableReference;
        private set
        {
            if (!string.IsNullOrEmpty(_readableReference))
                return;
            
            _readableReference = value;
        }
    }

    private Book _book;

    public Book Book
    {
        get => _book;
        private set
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
        private set
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
    
    public string ChapterId => Book.Abbreviation.ToUpper() 
                   + '.' 
                   + Chapter.ToString();


    public string VerseId
    {
        get
        {
            if (VerseNumbers is null || VerseNumbers.Count == 0)
                throw new InvalidOperationException("Unable to get VerseId: VerseNumbers is null or empty");
            
            return Book.Abbreviation.ToUpper() 
                + '.' 
                + Chapter.ToString() 
                + '.'
                + VerseNumbers.First().ToString();
        }
    }

    private List<int> _verseNumbers;

    public List<int> VerseNumbers
    {
        get => _verseNumbers;
        set
        {
            // If Reference was initiated without any verse numbers
            // Only set ReadableReference when the Reference has book, chapter, and verse number
            if (string.IsNullOrEmpty(_readableReference) && value.Count > 0 && Book is not null && Chapter != 0)
            {
                ReadableReference = ReferenceParser.ConvertToReadableReference(Book.DisplayName, Chapter, VerseNumbers);
            }

            _verseNumbers = new List<int>(value);
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
        
        ReadableReference = readableReference;
        
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

    public override string ToString()
    {
        if (string.IsNullOrEmpty(_readableReference))
            throw new InvalidOperationException(
                "Unable to get Reference ToString, _readableReference is null or empty");
        
        return _readableReference;
    }
}
