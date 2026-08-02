namespace ScriptureMemory.Server.Data.Dtos;

public sealed record ResponseChapterDto(
    string reference, 
    string contentUsx,
    string copyright);