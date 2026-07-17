namespace ScriptureMemory.Server.Services;

public class AuthorizationSyncerActive
{
    private int _isCurrentlySyncing = 0;

    private readonly BibleSyncer _syncer;

    public AuthorizationSyncerActive(
        BibleSyncer syncer)
    {
        _syncer = syncer;
    }

    public bool SetActive()
    {
        return Interlocked.CompareExchange(ref _isCurrentlySyncing, 1, 0) == 0
            ? true
            : false;
    }

    public void SetInactive()
    {
        Interlocked.Exchange(ref _isCurrentlySyncing, 0);
    }

    public bool IsCurrentlySyncing()
    {
        return Interlocked.CompareExchange(ref _isCurrentlySyncing, 0, 0) == 1 
            ? true 
            : false;
    }
}