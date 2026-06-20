using DataAccess.Models;
using J2N.Text;
using ScriptureMemory.Server.Data.Models;
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
    /// Parse an input of a user-typed reference into a reference object
    /// </summary>
    /// <param name="input">
    /// Accepted input formats:
    ///   - Psalms 119:2
    ///   - Psalms 119 2
    ///   - Psalms 119:2-5, 7-8, 10
    ///   - Minor book spelling typos
    /// </param>
    /// <returns>
    /// A Reference object with all its fields populated.
    /// Returns a null Reference if no valid reference could be parsed from the input.
    /// </returns>
    public static Reference? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        try
        {
            Reference returnReference = new();
            ReadOnlySpan<char> inputSpan = input.Trim().AsSpan();
            int i = 0;

            // Move up the book
            while (i < inputSpan.Length 
                   && (char.IsLetter(inputSpan[i])
                       || i <= 1)) // Move up for numbered books like "1 John"
            {
                i++;
            }

            // Check if a valid book
            Book? book = GetBook(inputSpan[..i].ToString().ToLower());
            if (book is null)
                return null;
            else
                returnReference.Book = book;

            // Move up to the chapter. We know the next digits after the book are the chapter.
            if (i < inputSpan.Length && !char.IsDigit(inputSpan[i]))
                i++;
            int chapterStart = i;
            
            // Move up to the end of the chapter
            while (i < inputSpan.Length && char.IsDigit(inputSpan[i]))
                i++;

            returnReference.Chapter = int.Parse(inputSpan[chapterStart..i]);

            // Move up to the start of the verses part
            if (i < inputSpan.Length && (!char.IsDigit(inputSpan[i]) || inputSpan[i] == ':'))
                i++;

            ReadOnlySpan<char> versesPartSpan = inputSpan[i..]; // The rest of the input should be the verses part

            string versesPart;
            int dashIndex = versesPartSpan.IndexOf('-');
            
            if (dashIndex >= 0)
            { // Handle a dash in the verses part
                ReadOnlySpan<char> firstPart = versesPartSpan[..dashIndex];
                ReadOnlySpan<char> secondPart = versesPartSpan[(dashIndex + 1)..];

                // Check if after the dash contains a book.
                // Handles formats like "Psalms 119:2-Psalms 119:3" that is used by the dataset used for cross-references
                bool secondPartContainsLetters = false;
                for (int l = 0; l < secondPart.Length; l++)
                {
                    if (char.IsLetter(secondPart[l]) // Handle a book name being after the dash
                        && secondPart.Length > 0)
                    {
                        secondPartContainsLetters = true;
                        break;
                    }
                }
                
                if (secondPart.Length > 0 && secondPartContainsLetters)
                {
                    // Move backwards, extracting the verses part and ignoring the book
                    int k = firstPart.Length - 1;
                    while (k >= 0 && char.IsDigit(firstPart[k]))
                        k--;
                    ReadOnlySpan<char> firstDigits = firstPart[(k + 1)..];

                    int m = secondPart.Length - 1;
                    while (m >= 0 && char.IsDigit(secondPart[m]))
                        m--;
                    ReadOnlySpan<char> secondDigits = secondPart[(m + 1)..];

                    // Replace versesPart 
                    versesPart = string.Concat(firstDigits, "-", secondDigits);
                }
                else
                {
                    versesPart = versesPartSpan.ToString();
                }

            }
            else
            {
                versesPart = versesPartSpan.ToString();
            }

            returnReference.VerseNumbers = GetIndividualVerses(versesPart, false);
            returnReference.ReadableReference = ConvertToReadableReference(
                returnReference.Book.DisplayName, 
                returnReference.Chapter, 
                returnReference.VerseNumbers);
            
            return returnReference;
        }
        catch (Exception e)
        {
            return null;
        }
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
        return new Reference(book, chapter, verses);
    }

    /// <summary>
    /// Convert a full reference into its parts for typing out during a practice session
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>List<string> { "Psalms", "119", "2", "4", "7" }</returns>
    public static List<string> GetVerseTypingParts(string reference)
    {
        var parts = new List<string>();

        string book = GetBook(reference).DisplayName;
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
    { // TODO: Refactor to use Span<T>
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
    /// <param name="reference">
    /// Accepts formats like:
    ///   - 2, 3
    ///   - 2-4
    ///   - 2-4, 6, 9-12
    /// </param>
    /// <param name="isFullReference">
    /// If the input reference is full ("Psalms 119:2-3") versus just the verses part ("2-3")
    /// </param>
    /// <returns>
    /// A list of all individual verses in the reference or verse part of a reference.
    /// Example: { 2, 3, 4, 6, 9, 10, 11, 12 }
    /// </returns>
    public static List<int> GetIndividualVerses(string reference, bool isFullReference = true)
    { // TODO: Refactor to use Span<T>
        List<int> returnList = new();
        string versesPart;
        
        if (isFullReference)
            versesPart = GetVersesHalfOfReference(reference);
        else
            versesPart = reference;

        foreach (string part in versesPart.Split(','))
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

        Reference? reference = Parse(referenceString);

        if (reference is null)
            throw new ArgumentException($"{referenceString} is not a valid reference.");

        foreach (var verseNumber in reference.VerseNumbers)
        {
            references.Add(ConvertToReferenceString(reference.Book.DisplayName, reference.Chapter, verseNumber));
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
    /// Gets a book's display name from a reference string
    /// </summary>
    /// <param name="reference"></param>
    /// <returns>"Psalms"</returns>
    public static Book? GetBook(string reference)
    {
        reference = reference.Trim();
        
        string[] parts = new string[1];
        Book? book;
        
        if (reference.Contains(' '))
            parts = reference.Split(' ');
        else
            parts[0] = reference;

        Books.TryGetBook(parts[0], out book);
        
        if (book is null)
        { // Handle books with one space in its name
            string bookWithNumber = parts[0] + " " + parts[1];
            Books.TryGetBook(bookWithNumber, out book);
            if (book is null)
            {
                // Handle books with two spaces in its name
                bookWithNumber = parts[0] + " " + parts[1] + " " + parts[2];
                Books.TryGetBook(bookWithNumber, out book);

                return book; // There are no valid book names with three or more spaces in its name
            }
            else
            {
                return book;
            }
        }
        else
        {
            return book;
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

        if (parts.Length > 1 && Books.TryGetBook(parts[0], out Book? book))
        {
            var chapterPart = parts[1].Split(':')[0];

            if (int.TryParse(chapterPart, out int chapter))
                return chapter;
            throw new Exception("Failed to parse chapter number.");
        }
        else
        {
            string bookWithNumber = parts[0] + " " + parts[1];

            if (Books.TryGetBook(bookWithNumber, out _))
            {
                var chapterPart = parts[2].Split(':')[0];
                if (int.TryParse(chapterPart, out int chapter))
                    return chapter;
                else
                    throw new Exception($"Failed to parse chapter number from reference: {reference}" +
                                        $" | parts[1]: {parts[1]}");
            }
            else
            {
                bookWithNumber = parts[0] + " " + parts[1] + " " + parts[2];

                if (Books.TryGetBook(bookWithNumber, out _))
                {
                    var chapterPart = parts[3].Split(':')[0];
                    if (int.TryParse(chapterPart, out int chapter))
                        return chapter;
                    else
                        throw new Exception($"Failed to parse chapter number from reference: {reference} " +
                                            $"| parts[3]: {parts[3]}");
                }
                else
                {
                    throw new Exception($"Failed to parse chapter number from reference: {reference} " +
                                        $"| parts[1]: {parts[1]}");
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
