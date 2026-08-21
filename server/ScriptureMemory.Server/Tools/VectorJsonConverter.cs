using Pgvector;
using System.Text.Json.Serialization;

namespace ScriptureMemory.Server.Tools;

public class VectorJsonConverter : JsonConverter<Vector>
{
    public override Vector? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        var floats = JsonSerializer.Deserialize<float[]>(ref reader, options);
        return floats is null ? null : new Vector(floats);
    }

    public override void Write(Utf8JsonWriter writer, Vector value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToArray(), options);
    }
}