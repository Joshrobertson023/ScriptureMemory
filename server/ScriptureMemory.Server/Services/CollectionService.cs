using DataAccess.DataInterfaces;
using DataAccess.Models;
using VerseAppNew.Server.Services;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.Server.Services;

public interface ICollectionService
{
    Task<int> CreateCollection(Collection newCollection);
}

public sealed class CollectionService : ICollectionService
{
    private readonly ICollectionData collectionContext;
    private readonly IActivityLogger logger;

    public CollectionService(
        ICollectionData collectionContext, 
        IActivityLogger logger)
    {
        this.collectionContext = collectionContext;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new collection for a user
    /// </summary>
    /// <param name="newCollection"></param>
    /// <returns>int, the new collection's id</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<int> CreateCollection(Collection newCollection)
    {
        if (string.IsNullOrEmpty(newCollection.Title))
            throw new ArgumentException("Cannot create collection: Title is required.");

        // Get the next order position to insert the new collection at the bottom of the list
        newCollection.OrderPosition = await collectionContext.GetNextOrderPosition(newCollection.UserId);

        // Ensure no duplicate favorites collection is created
        if (newCollection.IsFavorites)
        {
            if (await collectionContext.FavoritesExists(newCollection.UserId))
                throw new InvalidOperationException("Cannot create collection: Favorites collection already exists for user.");
        }

        // Ensure not saving from a published collection
        if (newCollection.PublishedId is not null || newCollection.AuthorId is not null)
            throw new ArgumentException("Cannot create collection: PublishedId or AuthorId should be null for user-created collection.");

        // Default progress to null
        newCollection.ProgressPercent = null;

        int newCollectionId = await collectionContext.InsertCollection(newCollection);

        await logger.Log(
            new ActivityLog(
                newCollection.UserId,
                ActionType.Create,
                EntityType.Collection,
                newCollectionId,
                "User created collection",
                new
                {
                    IsFavorites = newCollection.IsFavorites,
                }
            )
        );

        return newCollectionId;
    }

    //public async Task<int> SaveCollection(Collection collection)
    //{
    //    if (await collectionContext.SavedFromPublishedExists(collection))
    //        throw new InvalidOperationException("Cannot create collection: User already saved collection.");

    //    // Get the author's id
    //    //if (newCollection.AuthorId is null)
    //    //    newCollection.AuthorId = await collectionContext.GetAuthorIdForPublishedCollection(newCollection.PublishedId);

    //    // Saved collection should not be favorites
    //    collection.IsFavorites = false;
    //}
}
