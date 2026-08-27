using ScriptureMemory.Server.Data.Dtos;

namespace ScriptureMemory.Server.Data.DtoMappings;

public static class ChapterMappingExtensions
{
    public static ResponseChapterDto ToDto(this Chapter chapter)
    {
        return new ResponseChapterDto(
            new Models.Book(),
            chapter.Reference.Chapter,
            new List<Verse>() { },
            "",
            "");
    }
}