namespace ScriptureMemory.Server.Tools;

public static class BibleData
{
    public static readonly List<Bible> Bibles = new()
    {
        new Bible
        {
            Id = "de4e12af7f28f599-01",
            Version = "kjv"
        },
        new Bible
        {
            Id = "63097d2a0a2f7db3-01",
            Version = "nkjv"
        },
        new Bible
        {
            Id = "78a9f6124f344018-01",
            Version = "niv"
        },
        new Bible
        {
            Id = "b8ee27bcd1cae43a-01",
            Version = "nasb"
        }
    };

    public class Bible
    {
        public string Id { get; set; } = "";
        public string Version { get; set; } = "";
    }
}