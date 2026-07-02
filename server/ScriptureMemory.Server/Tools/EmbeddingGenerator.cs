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
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Services;
using System.ClientModel;
using System.Net;

namespace ScriptureMemory.Server.Tools;

public class EmbeddingGenerator
{
    private readonly IVerseData _verseData;
    private readonly string _apiKey;

    public EmbeddingGenerator(
        IVerseData verseData,
        IConfiguration config)
    {
        _verseData = verseData;
        _apiKey = config["OpenAi:OPENAI_API_KEY"]!;
    }

    public async Task<Vector> GenerateEmbedding(string input)
    {
        EmbeddingClient client = new("text-embedding-3-small", new ApiKeyCredential(_apiKey));
        OpenAIEmbedding embedding = client.GenerateEmbedding(input);
        return new Vector(embedding.ToFloats().ToArray());
    }

    public async Task<List<Vector>> GenerateEmbeddings(List<string> inputs)
    {
        EmbeddingClient client = new("text-embedding-3-small", new ApiKeyCredential(_apiKey));
        OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync(inputs);
        return embeddings.Select(e => new Vector(e.ToFloats().ToArray())).ToList();
    }

    public async Task<Vector> GetVerseContentEmbedding(VerseTranslationContent content)
    {
        return await GenerateEmbedding(content.GetEmbeddingText()
            ?? throw new InvalidOperationException("The verse content was not initialized sufficiently."));
    }

    // [Obsolete("Only used once")]
    // public async Task GenerateAllVerseEmbeddings()
    // {
    //     var allVerses = await _verseData.GetAllVerses();
    //
    //     foreach (var verse in allVerses.Where(v => v.Content == null))
    //     {
    //         Vector embedding = await GenerateVerseEmbedding(verse);
    //         verse.Content.Embedding = embedding;
    //
    //         await _verseData.InsertEmbedding(verse);
    //     }
    // }
}
