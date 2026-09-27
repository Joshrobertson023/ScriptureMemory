using System.Text.Json.Serialization;

public class JsonContent
{
    public string? Name { get; set; }
    public string? Text { get; set; }
    public string? Type { get; set; }

    [JsonPropertyName("attrs")]
    public Dictionary<string, JsonElement>? Attrs { get; set; }

    [JsonPropertyName("items")]
    public List<JsonContent>? Items { get; set; }
}