namespace ScriptureMemory.Server.Services;

public class AuthorizationSyncerData
{
    private int _isCurrentlySyncing = 0;

    public AuthorizationSyncerData()
    {
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