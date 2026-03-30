//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using System.Diagnostics;
//using ScriptureMemory.Server.Tools;

//namespace VerseAppNew.Server.Services;

//public sealed class UserPassageService
//{
//    private readonly IUserPassageData passageContext;
//    private readonly IVerseData verseContext;

//    public UserPassageService(
//        IUserPassageData passageContext, 
//        IVerseData verseContext)
//    {
//        this.passageContext = passageContext;
//        this.verseContext = verseContext;
//    }

//    // PassageParts is used for a practice session
//    public async Task<IResult> GetUserPassageParts(UserPassage passage)
//    {
//        if (string.IsNullOrEmpty(passage.Reference.ReadableReference))
//            return Results.BadRequest("ReadableReference is required");

//        string book = ReferenceParser.GetBook(passage.Reference.ReadableReference);
//        int chapter = ReferenceParser.GetChapter(passage.Reference.ReadableReference);
//        List<string> verseParts = ReferenceParser.GetVerseTypingParts(passage.Reference.ReadableReference);
//        List<string> references = ReferenceParser.GetReferencesFromVersesInReference(passage.Reference.ReadableReference);
//        string text = await passageContext.GetPassageTextFromListOfReferences(references);

//        var userVerseParts = new PassageParts
//        {
//            Book = book,
//            Chapter = chapter,
//            VerseParts = verseParts,
//            Text = text
//        };

//        return Results.Ok(userVerseParts);
//    }
//}
