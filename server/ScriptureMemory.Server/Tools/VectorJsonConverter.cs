using Pgvector;
using System.Text.Json.Serialization;

namespace ScriptureMemory.Server.Tools;

/// <summary>
/// This class converts Vector (embeddings) into byte[] to store in Redis cache
/// </summary>
public class VectorJsonConverter : JsonConverter<Vector>
{
    public override Vector? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            string? base64String = reader.GetString();
            if (string.IsNullOrEmpty(base64String)) return null;

            byte[] bytes = System.Convert.FromBase64String(base64String);

            float[] floats = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);

            return new Vector(floats);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = new List<float>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray) break;
                list.Add(reader.GetSingle());
            }
            return new Vector(list.ToArray());
        }

        throw new JsonException($"Unexpected token structural type '{reader.TokenType}' when deserializing Pgvector.");
    }

    public override void Write(Utf8JsonWriter writer, Vector value, JsonSerializerOptions options)
    {
        float[] floats = value.ToArray();

        byte[] bytes = new byte[floats.Length * sizeof(float)];
        Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);

        string base64String = System.Convert.ToBase64String(bytes);
        writer.WriteStringValue(base64String);
    }
}