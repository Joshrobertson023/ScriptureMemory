using DataAccess.DataInterfaces;

namespace ScriptureMemory.Server.Services;

public interface ICollectionService
{

}

public sealed class CollectionService : ICollectionService
{
    private readonly ICollectionData collectionContext;

    public CollectionService(ICollectionData collectionContext)
    {
        this.collectionContext = collectionContext;
    }


}
