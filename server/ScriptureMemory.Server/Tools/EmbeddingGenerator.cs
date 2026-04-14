using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using BenchmarkDotNet.Configs;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Embeddings;
using Pgvector;
using ScriptureMemory.Server.Services;
using System.ClientModel;
using System.Net;

namespace ScriptureMemory.Server.Tools;

public class EmbeddingGenerator
{
    private readonly VerseData _verseData;
    private readonly string _apiKey;

    public EmbeddingGenerator(
        VerseData verseData,
        IConfiguration config)
    {
        _verseData = verseData;
        _apiKey = config["OpenAi:OPENAI_API_KEY"]!;
    }

    public async Task<float[]> GetEmbedding(string input)
    {
        EmbeddingClient client = new("text-embedding-3-small", new ApiKeyCredential(_apiKey));
        OpenAIEmbedding embedding = client.GenerateEmbedding(input);
        return embedding.ToFloats().ToArray();
    }

    public async Task<Vector> GenerateEmbedding(Verse verse)
    {
        return new Vector(
            await GetEmbedding(
                verse.GetEmbeddingText()));
    }

    public async Task GenerateAllVerseEmbeddings()
    {
        var allVerses = await _verseData.GetAllVerses();

        foreach (var verse in allVerses)
        {
            Vector embedding = await GenerateEmbedding(verse);
            verse.Embedding = embedding;

            await _verseData.InsertEmbedding(verse);
        }
    }
}
