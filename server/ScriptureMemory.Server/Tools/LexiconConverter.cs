using Newtonsoft.Json;
using System.Xml;

namespace ScriptureMemory.Server.Tools;

public class LexiconConverter
{
    public async Task ConvertToJson()
    {
        XmlDocument xml = new();

        xml.Load(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\Lexicons\abbott-smith.tei.xml");

        using StreamWriter writer = File.CreateText(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\Lexicons\abbott-smith.tei.json");
        JsonSerializer serializer = new JsonSerializer
        {
            Formatting = Newtonsoft.Json.Formatting.Indented
        };
        serializer.Serialize(writer, xml);
    }
}
