using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface IUserPassageData
{
    Task<string> GetPassageTextFromListOfReferences(List<string> references);
    Task<List<UserPassage>> GetUserPassagesPopulatedForCollection(int collectionId);
}
