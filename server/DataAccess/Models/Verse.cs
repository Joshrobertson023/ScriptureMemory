using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;
using static DataAccess.Data.VerseData;

namespace DataAccess.Models;
public class Verse
{
    public int Id { get; set; } = 0;
    public Reference Reference { get; set; }
    public string Text { get; set; }
    public int UsersSavedCount { get; set; } = 0;
    public int UsersMemorizedCount { get; set; } = 0;
    public string VerseNumbers { get; set; } // Typable part of reference that has verses

    public Verse() { }

    public Verse(Reference reference, string text, string verseNumbers)
    {
        Reference = reference;
        Text = text;
        VerseNumbers = verseNumbers;
    }

    public Verse(Reference reference, string text)
    {
        Reference = reference;
        Text = text;
        VerseNumbers = ReferenceParse.GetVersesHalfOfReference(this.Reference.ReadableReference);
    }

    public Verse(VerseRow row)
    {
        Id = row.Id;
        Reference = new Reference(row.Reference);
        Text = row.Text;
        UsersSavedCount = row.UsersSavedCount;
        UsersMemorizedCount = row.UsersMemorizedCount;
        VerseNumbers = ReferenceParse.GetVersesHalfOfReference(this.Reference.ReadableReference);
    }
}
