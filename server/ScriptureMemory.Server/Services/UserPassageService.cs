using DataAccess.DataInterfaces;
using DataAccess.Models;
using System.Diagnostics;
using ScriptureMemoryLibrary;

namespace VerseAppNew.Server.Services;

public interface IUserPassageService
{
    Task<IResult> GetUserPassageParts(UserPassage passage);
}

public sealed class UserPassageService : IUserPassageService
{
    private readonly IUserPassageData passageContext;
    private readonly IVerseData verseContext;

    public UserPassageService(
        IUserPassageData passageContext, 
        IVerseData verseContext)
    {
        this.passageContext = passageContext;
        this.verseContext = verseContext;
    }

    // PassageParts is used for a practice session
    public async Task<IResult> GetUserPassageParts(UserPassage passage)
    {
        if (string.IsNullOrEmpty(passage.Reference.ReadableReference))
            return Results.BadRequest("ReadableReference is required");

        string book = ReferenceParse.GetBook(passage.Reference.ReadableReference);
        int chapter = ReferenceParse.GetChapter(passage.Reference.ReadableReference);
        List<string> verseParts = ReferenceParse.GetVerseTypingParts(passage.Reference.ReadableReference);
        List<string> references = ReferenceParse.GetReferencesFromVersesInReference(passage.Reference.ReadableReference);
        string text = await passageContext.GetPassageTextFromListOfReferences(references);

        var userVerseParts = new PassageParts
        {
            Book = book,
            Chapter = chapter,
            VerseParts = verseParts,
            Text = text
        };

        return Results.Ok(userVerseParts);
    }
}
