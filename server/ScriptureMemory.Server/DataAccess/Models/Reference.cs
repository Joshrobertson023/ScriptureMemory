using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;

namespace DataAccess.Models;

public sealed class Reference
{
    public string ReadableReference { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Book { get; set; } = string.Empty;
    
    public int Chapter { get; set; }
    
    public List<int> VerseNumbers { get; set; } = new();
    
    public Reference() { }

    public Reference(string readableReference)
    {
        string? book = ReferenceParser.GetBook(readableReference);
        Book = book is null
            ? throw new ArgumentNullException(nameof(book))
            : book;
        
        ReadableReference = readableReference;
        
        Chapter = ReferenceParser.GetChapter(readableReference);
        
        VerseNumbers = ReferenceParser.GetIndividualVerses(readableReference);
    }

    public Reference(string book, int chapter, List<int> verseNumbers)
    {
        if (!Books.TryGetBook(book, out _))
            throw new ArgumentException($"Book {book} not found");
        
        ReadableReference = ReferenceParser.ConvertToReadableReference(Book, Chapter, verseNumbers);
        Book = book;
        Chapter = chapter;
        VerseNumbers = verseNumbers;
    }

    public override string ToString()
    {
        return ReferenceParser.ConvertToReadableReference(Book, Chapter, VerseNumbers);
    }
}
