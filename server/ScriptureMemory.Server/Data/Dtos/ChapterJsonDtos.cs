using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Data.Dtos;

public sealed record ChapterSpanDto(
    string Text,
    string VerseId,
    int VerseNumber,
    bool IsVerseStart,
    bool Italic);

public sealed record ChapterParagraphDto(
    string Style,
    List<ChapterSpanDto> Spans);

public sealed record ChapterVerseJsonDto(
    string VerseId,
    int VerseNumber,
    string Text);

public sealed record ResponseChapterJsonDto(
    Book Book,
    int ChapterNumber,
    List<ChapterParagraphDto> Paragraphs,
    List<ChapterVerseJsonDto> Verses,
    string Copyright);

public sealed record ResponseVerseJsonDto(
    string VerseId,
    int VerseNumber,
    List<ChapterSpanDto> Spans,
    string Text);
