using static ScriptureMemoryLibrary.Enums;

namespace VerseAppNew.Server.Requests;

public class UpdateBibleVersionRequest
{
    public int UserId { get; set; }
    public BibleVersion BibleVersion { get; set; }
}
