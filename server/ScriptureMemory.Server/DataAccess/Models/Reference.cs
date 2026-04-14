using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;

namespace DataAccess.Models;

public sealed class Reference
{
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public List<int> Verses { get; set; } = new();
    public string ReadableReference { get; set; } = string.Empty;

    public override string ToString()
    {
        return ReferenceParser.ConvertToReadableReference(Book, Chapter, Verses);
    }

    public Reference() { }
}
