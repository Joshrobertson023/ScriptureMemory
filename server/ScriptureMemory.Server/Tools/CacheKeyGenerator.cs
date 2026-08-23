namespace ScriptureMemory.Server.Tools;

public static class CacheKeyGenerator
{
    public static string GetVerseCacheKey(string verseId, string translation)
    {
        return verseId + translation.Trim().ToLower();
    }

    public static string GetVerseCacheKey(Reference reference, string translation)
    {
        return reference.CacheKey + translation.Trim().ToLower();
    }
}
