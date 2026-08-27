namespace ScriptureMemory.Server.Tools;

/// <summary>
/// To avoid purging the Redis cache as much as possible, this class
/// ensures only items that meet requirements get cached, so even
/// if there is a bug, it skips caching the item
/// </summary>
public static class CacheValidator
{
    public static bool IsGoodToCache(this Verse verse)
    {
        bool isGoodToCache = false;

        if (verse.TranslationContents is not null &&
            verse.TranslationContents.Count == 1 &&
            (!string.IsNullOrEmpty(verse.TranslationContents.First().Version) &&
            ((!string.IsNullOrEmpty(verse.TranslationContents.First().PlainText) || (!string.IsNullOrEmpty(verse.TranslationContents.First().ContentUsx))))))
        {
            isGoodToCache = true;
        }

        return isGoodToCache;
    }
}
