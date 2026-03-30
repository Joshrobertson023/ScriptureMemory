using DataAccess.Data;
using DataAccess.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScriptureMemory.Server.Tools;

public static class CleanVerses
{
    public static Verse CleanVerse(Verse verse)
    {
        string cleanedText = verse.Text
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("\t", " ");

        // Collapse multiple consecutive spaces into one
        cleanedText = Regex.Replace(cleanedText, @" {2,}", " ");

        cleanedText = cleanedText.Trim();

        if (cleanedText != verse.Text)
        {
            verse.Text = cleanedText;
        }

        return verse;
    }
}
