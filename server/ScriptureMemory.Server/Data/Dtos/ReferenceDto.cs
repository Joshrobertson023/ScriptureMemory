namespace ScriptureMemory.Server.Data.Dtos;

public sealed record ReferenceDto(string readableReference, string book, int chapter, List<int>? verseNumbers);