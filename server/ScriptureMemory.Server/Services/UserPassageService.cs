using DataAccess.DataInterfaces;
using DataAccess.Models;
using System.Diagnostics;
using VerseAppLibrary;

namespace VerseAppNew.Server.Services;

public interface IUserPassageService
{
    Task<IResult> GetUserPassageParts(UserPassage passage);
}

public sealed class UserPassageService : IUserPassageService
{
    private readonly IUserPassageData passageContext;
    private readonly DataAccess.DataInterfaces.IVerseData verseContext;

    public UserPassageService(IUserPassageData passageContext, DataAccess.DataInterfaces.IVerseData verseContext)
    {
        this.passageContext = passageContext;
        this.verseContext = verseContext;
    }

    public async Task<IResult> GetUserPassageParts(UserPassage passage)
    {
        if (string.IsNullOrEmpty(passage.Reference.ReadableReference))
            return Results.BadRequest("ReadableReference is required");

        var book = ReferenceParse.GetBook(passage.Reference.ReadableReference);
        var chapter = ReferenceParse.GetChapter(passage.Reference.ReadableReference);
        var references = ReferenceParse.GetReferencesFromVersesInReference(passage.Reference.ReadableReference);
        var verseParts = ReferenceParse.GetVerseTypingParts(passage.Reference.ReadableReference);
        var text = ""; //await verseContext.GetPassageTextFromListOfReferences(references);

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
