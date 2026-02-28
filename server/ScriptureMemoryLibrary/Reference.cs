using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemoryLibrary;

public sealed class Reference
{
    public string Book { get; set; }
    public int Chapter { get; set; }
    public List<int> Verses { get; set; }
    private string _readableReference;
    public string ReadableReference
    {
        get => _readableReference;
        set => _readableReference = ReferenceParse.NormalizeReadableReference(value);
    }

    public Reference(string book, int chapter, List<int> verses)
    {
        Book = book;
        Chapter = chapter;
        Verses = verses;
        _readableReference = ReferenceParse.ConvertToReadableReference(Book, Chapter, Verses);
    }

    public Reference(string readableReference)
    {
        ReadableReference = readableReference;
        Book = ReferenceParse.GetBook(readableReference);
        Chapter = ReferenceParse.GetChapter(readableReference);
        Verses = ReferenceParse.GetIndividualVerses(readableReference);
    }
}
