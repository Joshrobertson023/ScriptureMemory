using static ScriptureMemoryLibrary.Enums;

namespace VerseAppNew.Server.Requests;

public class UpdateCollectionsSortRequest
{
    public int UserId { get; set; }
    public CollectionsSort SortBy { get; set; }
}
