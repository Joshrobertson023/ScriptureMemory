using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;

namespace DataAccess.Models;

public sealed class Reference
{
    public string Book { get; set; }
    public int Chapter { get; set; }
    public List<int> Verses { get; set; }
    public string ReadableReference { get; set; }

    public Reference(string book, int chapter, List<int> verses)
    {
        Book = book;
        Chapter = chapter;
        Verses = verses;
        ReadableReference = ReferenceParse.ConvertToReadableReference(Book, Chapter, Verses);
    }
}
