using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemoryLibrary;
public static class ReferenceParse
{
    /// <summary>
    /// Convert a full reference into individual parts as strings
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>Reference { Book = "Psalms", Chapter = 119, List<string> Verses = "2-4" }</returns>
    public static Reference ConvertStringToReference(string referenceString)
    {
        Reference reference = new(referenceString);

        reference.Book = GetBook(referenceString);
        reference.Chapter = GetChapter(referenceString);
        reference.Verses = GetIndividualVerses(referenceString);
        reference.ReadableReference = ConvertToReadableReference(reference.Book, reference.Chapter, reference.Verses);

        return reference;
    }

    /// <summary>
    /// Convert a full reference into its parts for typing out during a practice session
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>List<string> { "Psalms", "119", "2", "4", "7" }</returns>
    public static List<string> GetVerseTypingParts(string reference)
    {
        var parts = new List<string>();

        string book = GetBook(reference);
        int chapter = GetChapter(reference);

        parts.Add(book);
        parts.Add(chapter.ToString());

        string versesPart = GetVersesHalfOfReference(reference);

        foreach (var segment in versesPart.Split(','))
        {
            string trimmed = segment.Trim();

            if (trimmed.Contains('-'))
            {
                var range = trimmed.Split('-');
                parts.Add(range[0].Trim());
                parts.Add(range[1].Trim());
            }
            else
            {
                parts.Add(trimmed);
            }
        }

        return parts;
    }

    /// <summary>
    /// Convert a verse's parts into a full reference
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verse"></param>
    /// <returns>"Psalms 119:2"</returns>
    public static string ConvertToReferenceString(string book, int chapter, int verse)
    {
        StringBuilder returnString = new();

        returnString.Append(book).Append(" ").Append(chapter.ToString()).Append(":");

        returnString.Append(verse.ToString());

        return returnString.ToString();
    }

    /// <summary>
    /// Convert a verse's parts into a full reference with comma-separated verses
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verses"></param>
    /// <returns>"Psalms 119:2,3,4,7"</returns>
    public static string ConvertToReferenceString(string book, int chapter, List<int> verses)
    {
        StringBuilder returnString = new();

        returnString.Append(book).Append(" ").Append(chapter.ToString()).Append(":");

        if (verses.Count > 1)
        {
            for (int i = 0; i < verses.Count; i++)
            {
                returnString.Append(verses[i].ToString());
                if (i < verses.Count - 1)
                    returnString.Append(",");
            }
        }
        else
        {
            returnString.Append(verses[0].ToString());
        }

        return returnString.ToString();
    }

    /// <summary>
    /// Convert a verse's parts into a human-readable reference
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verses"></param>
    /// <returns>"Psalms 119:2-4, 7"</returns>
    public static string ConvertToReadableReference(string book, int chapter, List<int> verses)
    {
        if (verses == null || verses.Count == 0)
            return string.Empty;

        verses.Sort();
        var returnString = new StringBuilder();
        returnString.Append(book).Append(' ').Append(chapter).Append(':');

        int i = 0;
        while (i < verses.Count)
        {
            if (i > 0)
                returnString.Append(", ");

            int rangeStart = verses[i];
            int rangeEnd = rangeStart;

            while (i + 1 < verses.Count && verses[i + 1] == verses[i] + 1)
            {
                i++;
                rangeEnd = verses[i];
            }

            returnString.Append(rangeStart);
            if (rangeEnd > rangeStart)
                returnString.Append('-').Append(rangeEnd);

            i++;
        }

        return returnString.ToString();
    }

    /// <summary>
    /// Get a list of verse numbers from a full reference
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>List<int> { 2, 3, 4, 7 }/returns>
    public static List<int> GetIndividualVerses(string reference)
    {
        List<int> returnList = new List<int>();
        string verses = GetVersesHalfOfReference(reference);

        foreach (string part in verses.Split(','))
        {
            string trimmed = part.Trim();

            if (trimmed.Contains('-'))
            {
                string[] range = trimmed.Split('-');
                int start = int.Parse(range[0].Trim());
                int end = int.Parse(range[1].Trim());
                for (int i = start; i <= end; i++)
                    returnList.Add(i);
            }
            else
            {
                returnList.Add(int.Parse(trimmed));
            }
        }

        return returnList;
    }

    /// <summary>
    /// Get full readable references for each verse from a reference
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>
    /// List<int> { "Psalms 119:2", "Psalms 119:3", "Psalms 119:4", "Psalms 119:7" }
    /// </returns>
    public static List<string> GetReferencesFromVersesInReference(string referenceString)
    {
        List<string> references = new();

        Reference reference = ConvertStringToReference(referenceString);

        foreach (var verseNumber in reference.Verses)
        {
            references.Add(ConvertToReferenceString(reference.Book, reference.Chapter, verseNumber));
        }

        return references;
    }

    //public static List<string> GetIndividualVersesFromReference(string reference)
    //{
    //    List<string> references = new();

    //    if (string.IsNullOrWhiteSpace(reference))
    //        return references;

    //    reference = reference.Trim();
    //    reference = reference.Replace(" :", ":").Replace(": ", ":").Replace("  ", " ");

    //    int colonIndex = reference.IndexOf(':');
    //    if (colonIndex > 0)
    //    {
    //        if (colonIndex > 0 && !char.IsWhiteSpace(reference[colonIndex - 1]))
    //        {
    //            int lastSpace = reference.LastIndexOf(' ', colonIndex);
    //            if (lastSpace == -1)
    //            {
    //                for (int i = colonIndex - 1; i >= 0; i--)
    //                {
    //                    if (char.IsDigit(reference[i]))
    //                    {
    //                        reference = reference.Insert(i, " ");
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    var match = System.Text.RegularExpressions.Regex.Match(reference, @"^(?<book>[\dA-Za-z\s]+)\s+(?<chapter>\d+):(?<verses>[\d\-\,\s]+)$");
    //    if (!match.Success)
    //        return references;

    //    string book = match.Groups["book"].Value.Trim();
    //    string chapter = match.Groups["chapter"].Value.Trim();
    //    string versePart = match.Groups["verses"].Value.Trim();

    //    var segments = versePart.Split(',', StringSplitOptions.RemoveEmptyEntries);
    //    foreach (var seg in segments)
    //    {
    //        string part = seg.Trim();
    //        if (part.Contains('-'))
    //        {
    //            var bounds = part.Split('-', StringSplitOptions.RemoveEmptyEntries);
    //            if (bounds.Length == 2 &&
    //                int.TryParse(bounds[0], out int start) &&
    //                int.TryParse(bounds[1], out int end) &&
    //                end >= start)
    //            {
    //                for (int verse = start; verse <= end; verse++)
    //                    references.Add($"{book} {chapter}:{verse}");
    //            }
    //        }
    //        else if (int.TryParse(part, out int verse))
    //        {
    //            references.Add($"{book} {chapter}:{verse}");
    //        }
    //    }

    //    return references;
    //}

    /// <summary>
    /// Convert a full reference to the verse number(s) after the semicolon
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>"2-4, 7"</returns>
    public static string GetVersesHalfOfReference(string reference)
    {
        string[] parts = reference.Split(':');
        if (parts.Length > 1)
            return parts[1].Trim();
        else
            return string.Empty;
    }

    /// <summary>
    /// Get the book from a full reference
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>"Psalms"</returns>
    public static string GetBook(string reference)
    {
        string[] parts = reference.Split(' ');
        if (Data.books.Contains(parts[0]))
            return parts[0];
        else
        {
            string bookWithNumber = parts[0] + " " + parts[1];
            if (Data.books.Contains(bookWithNumber))
                return bookWithNumber;
            else
                throw new Exception($"Book {bookWithNumber} not found.");
        }
    }

    /// <summary>
    /// Get the chapter from a full reference
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>A chapter int</returns>
    public static int GetChapter(string reference)
    {
        string[] parts = reference.Split(' ');
        if (parts.Length > 1 && Data.books.Contains(parts[0]))
        {
            var chapterPart = parts[1].Split(':')[0];
            if (int.TryParse(chapterPart, out int chapter))
                return chapter;
            throw new Exception("Failed to parse chapter number.");
        }
        else
        {
            string bookWithNumber = parts[0] + " " + parts[1];
            if (Data.books.Contains(bookWithNumber))
            {
                var chapterPart = parts[2].Split(':')[0];
                if (int.TryParse(chapterPart, out int chapter))
                    return chapter;
            }
            throw new Exception($"Failed to parse chapter number from reference: {reference} | parts[1]: {parts[1]}");
        }
    }

    /// <summary>
    /// Normalizes a reference's syntax
    /// </summary>
    /// <param name="readableReference"></param>
    /// <returns></returns>
    public static string NormalizeReadableReference(string readableReference)
    {
        var reference = ConvertStringToReference(readableReference);
        return reference.ReadableReference;
    }
}
