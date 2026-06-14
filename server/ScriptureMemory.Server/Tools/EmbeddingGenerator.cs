// using Azure;
// using Azure.AI.OpenAI;
// using Azure.Identity;
// using BenchmarkDotNet.Configs;
// using DataAccess.Data;
// using DataAccess.Models;
// using Microsoft.Extensions.AI;
// using OpenAI;
// using OpenAI.Embeddings;
// using Pgvector;
// using ScriptureMemory.Server.Services;
// using System.ClientModel;
// using System.Net;
//
// namespace ScriptureMemory.Server.Tools;
//
// public class EmbeddingGenerator
// {
//     private readonly VerseData _verseData;
//     private readonly string _apiKey;
//
//     public EmbeddingGenerator(
//         VerseData verseData,
//         IConfiguration config)
//     {
//         _verseData = verseData;
//         _apiKey = config["OpenAi:OPENAI_API_KEY"]!;
//     }
//
//     public async Task<Vector> GenerateEmbedding(string input)
//     {
//         EmbeddingClient client = new("text-embedding-3-small", new ApiKeyCredential(_apiKey));
//         OpenAIEmbedding embedding = client.GenerateEmbedding(input);
//         return new Vector(embedding.ToFloats().ToArray());
//     }
//
//     public async Task<List<Vector>> GenerateEmbeddings(List<string> inputs)
//     {
//         EmbeddingClient client = new("text-embedding-3-small", new ApiKeyCredential(_apiKey));
//         OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync(inputs);
//         return embeddings.Select(e => new Vector(e.ToFloats().ToArray())).ToList();
//     }
//
//     public async Task<Vector> GenerateVerseEmbedding(Verse verse)
//     {
//         return await GenerateEmbedding(verse.Content?.GetEmbeddingText()
//             ?? throw new Exception("verse.VerseContent is null"));
//     }
//
//     public async Task GenerateAllVerseEmbeddings()
//     {
//         var allVerses = await _verseData.GetAllVerses();
//
//         foreach (var verse in allVerses.Where(v => v.Content == null))
//         {
//             Vector embedding = await GenerateVerseEmbedding(verse);
//             verse.Content.Embedding = embedding;
//
//             await _verseData.InsertEmbedding(verse);
//         }
//     }
// }
