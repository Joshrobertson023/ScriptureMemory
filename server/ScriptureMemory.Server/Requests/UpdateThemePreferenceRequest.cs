using static ScriptureMemoryLibrary.Enums;

namespace VerseAppNew.Server.Requests;

public class UpdateThemePreferenceRequest
{
    public int UserId { get; set; }
    public ThemePreference Preference { get; set; }
}
