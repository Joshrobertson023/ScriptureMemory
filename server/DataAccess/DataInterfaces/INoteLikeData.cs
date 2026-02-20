namespace DataAccess.DataInterfaces;

public interface INoteLikeData
{
    Task LikeNote(int noteId, string username);
    Task UnlikeNote(int noteId, string username);
    Task<bool> HasUserLikedNote(int noteId, string username);
    Task<int> GetNoteLikeCount(int noteId);
    Task<Dictionary<int, bool>> GetUserLikesForNotes(List<int> noteIds, string username);
    Task<Dictionary<int, int>> GetLikeCountsForNotes(List<int> noteIds);
}































