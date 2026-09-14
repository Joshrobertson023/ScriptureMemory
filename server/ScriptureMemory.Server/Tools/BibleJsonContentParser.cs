using ScriptureMemory.Server.Data.Dtos;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ScriptureMemory.Server.Tools;

/// <summary>
/// Flattens the recursive api.bible json content-type tree (paragraphs containing verse markers,
/// character-style runs, and text nodes) into a shape the client can render directly: a list of
/// styled paragraphs made of spans, each span tagged with the verse it belongs to.
/// </summary>
public static class BibleJsonContentParser
{
    /// <summary>
    /// Parses a full chapter's json content into paragraphs (preserving formatting/style), the
    /// flattened per-verse text (mirroring what BibleService extracts from the plain-text
    /// content-type), and verses in the same cache-ready shape BibleService.GetChapter builds
    /// so callers can enqueue them for caching the same way.
    /// </summary>
    public static (List<ChapterParagraphDto> Paragraphs, List<ChapterVerseJsonDto> Verses, List<Verse> VersesToCache) ParseChapter(
        List<JsonContent> content,
        string translation)
    {
        var paragraphs = new List<ChapterParagraphDto>();
        var verseTexts = new Dictionary<string, StringBuilder>();
        var verseOrder = new List<string>();

        // Tracks the verse we're currently "inside" as we walk the tree, in document order, so
        // text nodes that don't carry their own attrs.verseId (some paragraph styles - e.g. "lh"
        // list headers used in genealogies - omit it) still get attributed to the right verse.
        string currentVerseId = "";
        int currentVerseNumber = 0;

        foreach (var node in content)
        {
            // Every top-level node is walked, not just ones named "para" - some chapters wrap
            // verses in other top-level tags (e.g. "list", "table"), and skipping those silently
            // dropped their verses entirely.
            if (node.Type != "tag")
                continue;

            string style = GetAttr(node.Attrs, "style") ?? node.Name ?? "p";
            var spans = new List<ChapterSpanDto>();

            if (node.Items is not null)
            {
                FlattenItems(
                    node.Items,
                    spans,
                    verseTexts,
                    verseOrder,
                    italic: false,
                    ref currentVerseId,
                    ref currentVerseNumber);
            }

            paragraphs.Add(new ChapterParagraphDto(style, spans));
        }

        var verses = verseOrder
            .Select(id => new ChapterVerseJsonDto(
                id,
                ExtractVerseNumber(id),
                CleanVerseText(verseTexts[id].ToString())))
            .ToList();

        // Cache-ready verses, built the same way BibleService.GetChapter builds them for the
        // plain-text content-type - verseId is already in the "GEN.1.1" id format, so it can be
        // used directly instead of re-deriving it (e.g. by parsing digits back out of the id).
        var versesToCache = verses
            .Select(v => new Verse
            {
                Id = v.VerseId,
                TranslationContents = new List<VerseTranslationContent>
                {
                    new VerseTranslationContent
                    {
                        PlainText = v.Text,
                        Version = translation,
                        VerseId = v.VerseId,
                    }
                }
            })
            .ToList();

        return (paragraphs, verses, versesToCache);
    }

    /// <summary>
    /// Parses a single verse's json content (as returned by the /verses/{verseId} endpoint) into
    /// its spans (for formatting) and its flattened, whitespace-cleaned text.
    /// </summary>
    public static ResponseVerseJsonDto ParseVerse(JsonContent verseContent)
    {
        var spans = new List<ChapterSpanDto>();
        var verseTexts = new Dictionary<string, StringBuilder>();
        var verseOrder = new List<string>();
        string currentVerseId = "";
        int currentVerseNumber = 0;

        FlattenItems(
            new List<JsonContent> { verseContent },
            spans,
            verseTexts,
            verseOrder,
            italic: false,
            ref currentVerseId,
            ref currentVerseNumber);

        string verseId = verseOrder.FirstOrDefault() ?? "";
        string text = verseTexts.TryGetValue(verseId, out var builder) ? CleanVerseText(builder.ToString()) : "";

        return new ResponseVerseJsonDto(
            verseId,
            verseId.Length > 0 ? ExtractVerseNumber(verseId) : 0,
            spans,
            text);
    }

    private static void FlattenItems(
        List<JsonContent> items,
        List<ChapterSpanDto> spans,
        Dictionary<string, StringBuilder> verseTexts,
        List<string> verseOrder,
        bool italic,
        ref string currentVerseId,
        ref int currentVerseNumber)
    {
        foreach (var item in items)
        {
            if (item.Type == "tag" && item.Name == "verse")
            {
                // The "sid" attr looks like "GEN 1:1" - convert it to our "GEN.1.1" verse id format.
                string? sid = GetAttr(item.Attrs, "sid");
                string? number = GetAttr(item.Attrs, "number");
                string verseId = sid?.Replace(' ', '.').Replace(':', '.') ?? "";
                int verseNumber = int.TryParse(number, out var n) ? n : 0;

                if (verseId.Length > 0)
                {
                    EnsureVerseTracked(verseId, verseTexts, verseOrder);
                    currentVerseId = verseId;
                    currentVerseNumber = verseNumber;
                }

                spans.Add(new ChapterSpanDto(
                    Text: "",
                    VerseId: verseId,
                    VerseNumber: verseNumber,
                    IsVerseStart: true,
                    Italic: false));

                // Don't recurse into the verse tag's own items - it's just the verse number label.
            }
            else if (item.Type == "tag" && item.Name == "char")
            {
                string style = GetAttr(item.Attrs, "style") ?? "";
                bool isItalic = italic || style is "it" or "add" or "bd" or "bdit" or "em";

                if (item.Items is not null)
                    FlattenItems(item.Items, spans, verseTexts, verseOrder, isItalic, ref currentVerseId, ref currentVerseNumber);
            }
            else if (item.Type == "text")
            {
                string text = item.Text ?? "";

                // Fall back to whichever verse we're currently inside if this text node isn't
                // tagged itself - some paragraph styles omit the per-node verseId attr.
                string? verseId = GetAttr(item.Attrs, "verseId")
                    ?? (currentVerseId.Length > 0 ? currentVerseId : null);

                if (verseId is not null)
                {
                    EnsureVerseTracked(verseId, verseTexts, verseOrder);

                    // Append a trailing space after every fragment (harmless - CleanVerseText
                    // collapses runs of whitespace) so verses spanning multiple paragraphs (e.g.
                    // poetry lines) don't get glued together without a word boundary.
                    verseTexts[verseId].Append(text).Append(' ');
                }

                spans.Add(new ChapterSpanDto(
                    Text: text,
                    VerseId: verseId ?? "",
                    VerseNumber: verseId is not null ? ExtractVerseNumber(verseId) : 0,
                    IsVerseStart: false,
                    Italic: italic));
            }
            else if (item.Type == "tag" && item.Items is not null)
            {
                // Defensively recurse into any other tag type (e.g. unexpected/unknown styles)
                // so its text still makes it into the output instead of silently disappearing.
                FlattenItems(item.Items, spans, verseTexts, verseOrder, italic, ref currentVerseId, ref currentVerseNumber);
            }
        }
    }

    private static void EnsureVerseTracked(
        string verseId,
        Dictionary<string, StringBuilder> verseTexts,
        List<string> verseOrder)
    {
        if (verseId.Length == 0 || verseTexts.ContainsKey(verseId))
            return;

        verseTexts[verseId] = new StringBuilder();
        verseOrder.Add(verseId);
    }

    private static int ExtractVerseNumber(string verseId)
    {
        var parts = verseId.Split('.');
        return parts.Length > 0 && int.TryParse(parts[^1], out var n) ? n : 0;
    }

    private static string CleanVerseText(string text)
    {
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    private static string? GetAttr(Dictionary<string, JsonElement>? attrs, string key)
    {
        if (attrs is not null
            && attrs.TryGetValue(key, out var el)
            && el.ValueKind == JsonValueKind.String)
        {
            return el.GetString();
        }

        return null;
    }
}
