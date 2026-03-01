using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IPublishedCollectionData
{
    Task<int> GetAuthorId(int publishedId);
    Task<string> GetAuthorName(int publishedId);
    Task IncrementUsersSaved(int publishedId);
    Task<int> Insert(PublishedCollection collection);
    Task<PublishedCollection> Get(int publishedId);
}



