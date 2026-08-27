using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Data.Dtos;

public sealed record ResponseChapterDto(
    Book book,
    int chapterNumber,
    List<Verse> verses,
    string? title,
    string copyright);