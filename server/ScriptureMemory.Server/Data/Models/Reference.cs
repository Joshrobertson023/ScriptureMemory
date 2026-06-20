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
    public string ReadableReference { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public Book Book { get; set; }
    
    public int Chapter { get; set; }
    
    public List<int> VerseNumbers { get; set; } = new();
    
    public Reference() { }
    
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
        Book? book;
        
        bool succeeded = AllBooksInitializer.TryGetBook(
            ReferenceParser.GetBookName(readableReference), 
            out book);

        book = succeeded
            ? book
            : throw new InvalidOperationException($"{readableReference} is not a valid reference.");
        
        ReadableReference = readableReference;
        
        Chapter = ReferenceParser.GetChapter(readableReference);
        
        VerseNumbers = ReferenceParser.GetIndividualVerses(readableReference);
    }

    public Reference(string bookName, int chapter, List<int> verseNumbers)
    {
        if (!AllBooksInitializer.TryGetBook(bookName, out Book? book))
            throw new InvalidOperationException($"Book {bookName} not found");
        
        ReadableReference = ReferenceParser.ConvertToReadableReference(Book!.DisplayName, Chapter, verseNumbers);
        Book = book!;
        Chapter = chapter;
        VerseNumbers = verseNumbers;
    }

    public override string ToString()
    {
        return ReferenceParser.ConvertToReadableReference(Book!.DisplayName, Chapter, VerseNumbers);
    }
}
