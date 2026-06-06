using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;

namespace DataAccess.Models;

public sealed class Reference
{
    /// <summary>
    /// Passage reference Id ("PSA.1.1-PSA.1.3")
    /// </summary>
    public string Id { get; set; }
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public List<int> VerseNumbers { get; set; } = new();
    public string ReadableReference { get; set; } = string.Empty;

    public override string ToString()
    {
        return ReferenceParser.ConvertToReadableReference(Book, Chapter, VerseNumbers);
    }

    public Reference() { }

    public Reference(string id)
    {
        Id = id;
        Book = ReferenceParser.GetBook(Id);
    }
}
