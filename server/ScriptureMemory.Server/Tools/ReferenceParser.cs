using DataAccess.Models;
using J2N.Text;
using ScriptureMemory.Server.Files.CsvRecordModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemory.Server.Tools;
public static class ReferenceParser
{
    /// <summary>
    /// Convert an input into a reference object
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>Reference { Book = "Psalms", Chapter = 119, List<string> Verses = "2-4" }</returns>
    public static Reference Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input reference cannot be null or empty.");

        input = input.Trim();
        ReadOnlySpan<char> span = input.AsSpan();
        int i = 0;
        Reference returnReference = new();

        while (i < span.Length && (char.IsLetter(span[i]) || i <= 1))
            i++;

        //try
        //{
            returnReference.Book = GetBook(span[..i].ToString().ToLower());
        //}
        //catch (Exception)
        //{
        //    returnReference.Book = "Error parsing book";
        //    return returnReference;
        //}

        if (i < span.Length && !char.IsDigit(span[i]))
            i++;

        int chapterStart = i;
        while (i < span.Length && char.IsDigit(span[i]))
            i++;

        returnReference.Chapter = int.Parse(span[chapterStart..i]);

        if (i < span.Length && (!char.IsDigit(span[i]) || span[i] == ':'))
            i++;

        ReadOnlySpan<char> versesPartSpan = span[i..];

        string verses;
        int dashIndex = versesPartSpan.IndexOf('-');
        if (dashIndex >= 0)
        {
            ReadOnlySpan<char> firstPart = versesPartSpan[..dashIndex];
            ReadOnlySpan<char> secondPart = versesPartSpan[(dashIndex + 1)..];

            bool secondPartIsBook = false;
            for (int l = 0; l < secondPart.Length; l++)
            {
                if (!char.IsDigit(secondPart[l]))
                {
                    secondPartIsBook = true;
                    break;
                }
            }

            if (secondPart.Length > 0 && secondPartIsBook)
            {
                int k = firstPart.Length - 1;
                while (k >= 0 && char.IsDigit(firstPart[k]))
                    k--;
                ReadOnlySpan<char> firstDigits = firstPart[(k + 1)..];

                int m = secondPart.Length - 1;
                while (m >= 0 && char.IsDigit(secondPart[m]))
                    m--;
                ReadOnlySpan<char> secondDigits = secondPart[(m + 1)..];

                verses = string.Concat(firstDigits, "-", secondDigits);
            }
            else
            {
                verses = versesPartSpan.ToString();
            }
        }
        else
        {
            verses = versesPartSpan.ToString();
        }

        returnReference.Verses = GetIndividualVerses(verses, false);
        returnReference.ReadableReference = ConvertToReadableReference(returnReference.Book, returnReference.Chapter, returnReference.Verses);

        return returnReference;
    }

    /// <summary>
    /// Returns a Reference object from a book, chapter, and list of versess
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verses"></param>
    /// <returns></returns>
    public static Reference Parse(string book, int chapter, List<int> verses)
    {
        book = GetBook(book);
        return new Reference
        {
            Book = book,
            Chapter = chapter,
            Verses = verses,
            ReadableReference = ConvertToReadableReference(book, chapter, verses)
        };
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
    public static List<int> GetIndividualVerses(string reference, bool isFullReference = true)
    {
        List<int> returnList = new List<int>();
        string verses;
        if (isFullReference)
            verses = GetVersesHalfOfReference(reference);
        else
            verses = reference;

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

        Reference reference = Parse(referenceString);

        foreach (var verseNumber in reference.Verses)
        {
            references.Add(ConvertToReferenceString(reference.Book, reference.Chapter, verseNumber));
        }

        return references;
    }

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
        string[] parts = new string[1];
        if (reference.Contains(' '))
            parts = reference.Split(' ');
        else
            parts[0] = reference;
        if (Books.TryGetBook(parts[0], out string book))
            return book;
        else
        {
            string bookWithNumber = parts[0] + " " + parts[1];
            if (Books.TryGetBook(bookWithNumber, out book))
                return book;
            else
            {
                bookWithNumber = parts[0] + " " + parts[1] + " " + parts[2];
                if (Books.TryGetBook(bookWithNumber, out book))
                    return book;
                else
                    throw new Exception($"Book {bookWithNumber} not found.");
            }
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

        if (parts.Length > 1 && Books.TryGetBook(parts[0], out string book))
        {
            var chapterPart = parts[1].Split(':')[0];

            if (int.TryParse(chapterPart, out int chapter))
                return chapter;
            throw new Exception("Failed to parse chapter number.");
        }
        else
        {
            string bookWithNumber = parts[0] + " " + parts[1];

            if (Books.TryGetBook(bookWithNumber, out string _book))
            {
                var chapterPart = parts[2].Split(':')[0];
                if (int.TryParse(chapterPart, out int chapter))
                    return chapter;
                else
                    throw new Exception($"Failed to parse chapter number from reference: {reference} | parts[1]: {parts[1]}");
            }
            else
            {
                bookWithNumber = parts[0] + " " + parts[1] + " " + parts[2];

                if (Books.TryGetBook(bookWithNumber, out string __book))
                {
                    var chapterPart = parts[3].Split(':')[0];
                    if (int.TryParse(chapterPart, out int chapter))
                        return chapter;
                    else
                        throw new Exception($"Failed to parse chapter number from reference: {reference} | parts[3]: {parts[3]}");
                }
                else
                {
                    throw new Exception($"Failed to parse chapter number from reference: {reference} | parts[1]: {parts[1]}");
                }
            }
        }
    }

    /// <summary>
    /// Normalizes a reference's syntax
    /// </summary>
    /// <param name="readableReference"></param>
    /// <returns></returns>
    public static string NormalizeReadableReference(string readableReference)
    {
        var reference = Parse(readableReference);
        return reference.ReadableReference;
    }
}
