using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;

namespace DataAccess.Models;
public class Verse
{
    public int Id { get; set; } = 0;
    public string Reference { get; set; }
    public string Text { get; set; }
    public int UsersSavedCount { get; set; } = 0;
    public int UsersMemorizedCount { get; set; } = 0;
    public string VerseNumbers { get; set; }

    public Verse(string reference, string text, string verseNumbers)
    {
        Reference = reference;
        Text = text;
        VerseNumbers = verseNumbers;
    }

    public Verse(string reference, string text)
    {
        Reference = reference;
        Text = text;
        VerseNumbers = ReferenceParse.GetVersesHalfOfReference(this.Reference);
    }
}
