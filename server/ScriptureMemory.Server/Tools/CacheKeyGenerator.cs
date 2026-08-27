using ScriptureMemory.Server.Data.Dtos;

namespace ScriptureMemory.Server.Tools;

public static class CacheKeyGenerator
{
    public static string GetVerseCacheKey(
        string verseId,
        string translation,
        MemoryCacheType type)
    {
        return verseId + translation.Trim().ToLower() + type.ToString();
    }

    public static string GetVerseCacheKey(
        Reference reference,
        string translation,
        MemoryCacheType type)
    {
        return reference.CacheKey + translation.Trim().ToLower() + type.ToString();
    }

    public static MemoryCacheType GetCacheType(VerseTranslationContent content)
    {
        if (!string.IsNullOrEmpty(content.PlainText))
        {
            return MemoryCacheType.PlainText;
        }
        else if (!string.IsNullOrEmpty(content.ContentUsx))
        {
            return MemoryCacheType.Usx;
        }
        else
        {
            return MemoryCacheType.PlainText;
        }
    }

    public static string GetChapterDtoCacheKey(
        string chapterId, 
        string translation,
        MemoryCacheType type)
    {
        return chapterId + translation.Trim().ToLower() + type.ToString();
    }

    public static string GetChapterDtoCacheKey(
        Reference reference,
        string translation,
        MemoryCacheType type)
    {
        return reference.ChapterId + translation.Trim().ToLower() + type.ToString();
    }
}
