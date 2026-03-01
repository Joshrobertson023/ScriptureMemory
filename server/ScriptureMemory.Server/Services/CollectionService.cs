using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using VerseAppNew.Server.Services;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.Server.Services;

public interface ICollectionService
{
    Task<int> CreateCollection(Collection newCollection);
    Task<List<Collection>> GetUserCollections(int userId);
    Task SaveCollection(SaveCollectionRequest request);
    Task<Collection> GetCollection(Collection collection);
}

public sealed class CollectionService : ICollectionService
{
    private readonly ICollectionData collectionContext;
    private readonly IActivityLogger logger;
    private readonly IUserPassageData passageContext;

    public CollectionService(
        ICollectionData collectionContext, 
        IActivityLogger logger,
        IUserPassageData passageContext)
    {
        this.collectionContext = collectionContext;
        this.logger = logger;
        this.passageContext = passageContext;
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
        if (newCollection.UserId <= 0)
            throw new ArgumentException("Cannot create collection: UserId is required."); 

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

        // Insert passages and notes
        if (newCollection.Passages.Count > 0)
        {
            foreach (var passage in newCollection.Passages)
            {
                await passageContext.InsertUserPassage(passage);
            }
        }
        if (newCollection.Notes.Count > 0)
        {
            foreach (var note in newCollection.Notes)
            {
                await collectionContext.InsertCollectionNote(note);
            }
        }

        return newCollectionId;
    }

    /// <summary>
    /// Saves a collection for a user
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task SaveCollection(SaveCollectionRequest request)
    {
        if (await collectionContext.SavedFromPublishedExists(request.PublishedId, request.UserId))
            throw new InvalidOperationException("Cannot create collection: User already saved collection.");

        // Get the next order position to insert the new collection at the bottom of the list
        request.OrderPosition = await collectionContext.GetNextOrderPosition(request.UserId);

        await collectionContext.SaveCollection(request);
        // await publishedContext.IncrementUsersSaved(request.PublishedId);

        // Todo: Notify author

        await logger.Log(
            new ActivityLog(
                request.UserId,
                ActionType.Save,
                EntityType.Collection,
                null,
                "User saved collection",
                new
                {
                    PublishedId = request.PublishedId,
                }
            )
        );
    }

    /// <summary>
    /// Gets a list of empty user collections
    /// </summary>
    /// <param name="userId"></param>
    /// <returns><List<Collection>></returns>
    public async Task<List<Collection>> GetUserCollections(int userId)
    {
        List<Collection> collections = await collectionContext.GetUserCreatedCollections(userId);

        // Later on after doing published collections, call GetUserSavedCollections

        return collections;
    }

    /// <summary>
    /// Gets a user's collection filled in with passages and notes
    /// </summary>
    /// <param name="collectionId"></param>
    /// <param name="userId"></param>
    /// <returns>Collection</returns>
    public async Task<Collection> GetCollection(Collection collection)
    {
        if (collection.CollectionId <= 0)
            throw new InvalidOperationException("Cannot get collection: CollectionId is required.");

        collection.Passages = await passageContext.GetUserPassagesPopulatedForCollection(collection.CollectionId);
        collection.Notes = await collectionContext.GetCollectionNotes(collection.CollectionId);

        return collection;
    }
}
