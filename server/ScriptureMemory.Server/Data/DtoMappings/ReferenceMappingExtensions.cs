using ScriptureMemory.Server.Data.Dtos;

namespace ScriptureMemory.Server.Data.DtoMappings;

public static class ReferenceMappingExtensions
{
    public static ReferenceDto ToDto(this Reference reference)
    {
        return new ReferenceDto(
            reference.ReadableReference,
            reference.Book.DisplayName,
            reference.Chapter,
            reference.VerseNumbers
        );
    }
}