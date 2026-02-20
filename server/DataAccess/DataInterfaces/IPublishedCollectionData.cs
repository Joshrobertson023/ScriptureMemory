using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IPublishedCollectionData
{
    Task Publish(int collectionId, string? description);
    Task Unpublish(int collectionId);
    Task<PublishedCollection?> Get(int collectionId);
}



