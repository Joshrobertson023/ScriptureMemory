using CsvHelper;
using Dapper;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.Data.Sqlite;
using Npgsql;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Tools;
using System.Data;
using System.Globalization;
using Pgvector;

namespace ScriptureMemory.Server.Tools;

public sealed class VerseManagement
{
    private readonly VerseDataEfCore _verseData;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly ILogger<VerseManagement> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly EmbeddingGenerator _embeddingGenerator;

    public VerseManagement(
        VerseDataEfCore verseData,
        ApplicationDbContext context,
        IConfiguration config,
        ILogger<VerseManagement> logger,
        ApplicationDbContext dbContext,
        EmbeddingGenerator embeddingGenerator)
    {
        _verseData = verseData;
        _context = context;
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
        _logger = logger;
        _dbContext = dbContext;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task MoveVerses()
    {
        using var reader = new StreamReader(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\Verses\oracle.csv");
        using var writer = new StreamWriter(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\Verses\Kjv.csv");

        List<Verse> allVerses = new();
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Files.CsvRecordModels.OracleVerses>();
            foreach (var record in records)
            {
                // allVerses.Add(new Verse
                // {
                //     Id = record.VERSE_ID,
                //     Reference = ReferenceParser.Parse(record.VERSE_REFERENCE),
                //     Text = record.VERSE_TEXT,
                //     UsersSavedCount = record.USERS_SAVED_VERSE,
                //     UsersMemorizedCount = record.USERS_MEMORIZED
                // });
            }
        }

        List<Files.CsvRecordModels.Verse> csvVerses = new();

        foreach (var verse in allVerses)
        {
            string reference = verse.Reference.ToString();

            // csvVerses.Add(new Files.CsvRecordModels.Verse
            // {
            //     Id = verse.Id,
            //     Book = ReferenceParser.GetBook(reference),
            //     Chapter = ReferenceParser.GetChapter(reference),
            //     VerseNum = ReferenceParser.GetIndividualVerses(reference).FirstOrDefault(),
            //     Text = CleanVerses.CleanVerse(verse).Text
            // });
        }

        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(csvVerses);
        }

        _logger.LogDebug("Finished moving verses");
    }

    private List<Verse> CreateVersesAndRemoveDuplicates(List<ScriptureMemory.Server.Files.CsvRecordModels.Verse> verses)
    {
        HashSet<string> verseIds = new();
        List<string> duplicates = new();
        List<Verse> versesToReturn = new();
        
        foreach (var record in verses)
        {
            var verse = new Verse(record.Book, record.Chapter, record.VerseNum);
            verse.TranslationContents = new()
            {
                new VerseTranslationContent()
                {
                    PlainText = record.Text,
                    VerseNavigation = verse
                }
            };
            if (!verseIds.Add(verse.Id))
                duplicates.Add(verse.Id);
            else
                versesToReturn.Add(verse);
        }
        
        _logger.LogInformation("Found {count} duplicates: {duplicates}", duplicates.Count, duplicates);
        _logger.LogInformation("Remaining verses: {count}", versesToReturn.Count);
        
        return versesToReturn;
    }

    public async Task UploadVersesToPostgres()
    {
        List<Verse> versesInBatch = new();
        
        try
        {
            using var reader = new StreamReader(@"C:\Users\Josh Robertson\RiderProjects\ScriptureMemory\server\ScriptureMemory.Server\Files\Verses\Kjv.csv");

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var verses = CreateVersesAndRemoveDuplicates(csv.GetRecords<Files.CsvRecordModels.Verse>().ToList());
                
                const int batchSize = 300;
            
                for (int i = 0; ; i += batchSize)
                {
                    var batchedVerses = verses.Skip(i).Take(batchSize).ToList();
            
                    _logger.LogInformation("Requesting embeddings for {i}", i);
                    var embeddings = await _embeddingGenerator.GenerateEmbeddings(
                        batchedVerses.Select(v => v.TranslationContents.First().GetEmbeddingText()).ToList());

                    for (int j = 0; j < batchedVerses.Count; j++)
                    {
                        batchedVerses[j].TranslationContents.First().Embedding = embeddings[j];
                    }
                    
                    foreach (var batchedVerse in batchedVerses)
                    {
                        _ = batchedVerse.Reference.ReadableReference; // Call ReadableReference getter to populate it
                        versesInBatch.Add(batchedVerse);
                    }
                    
                    await _dbContext.Verses.AddRangeAsync(versesInBatch);
                    await _dbContext.SaveChangesAsync();
                    
                    versesInBatch.Clear();
            
                    if (((verses.Count) - i) <= batchSize)
                        break;
                }
            }
            
            _logger.LogInformation("Finished uploading verses to postgres");
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {e}", e.Message);
            _logger.LogError("Verses in batch: {0}", versesInBatch.Select(v => v.Id));
            throw;
        }
    }

    public class VerseDto
    {
        public int Id { get; set; }
        public string Book { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public int VerseNum { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    // public async Task UploadCrossReferences()
    // {
    //     string[] lines = File.ReadAllLines(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\CrossReferences\cross_references.txt");
    //     int total = lines.Length;
    //     int processed = 0;
    //
    //     _logger.LogDebug("Loading all verses into memory...");
    //     using var pgConn = new NpgsqlConnection(_connectionString);
    //     // var allVerses = (await pgConn.QueryAsync<VerseDto>(
    //     //     """
    //     //     select id as Id, book as Book, chapter as Chapter, text as Text, verse_num as VerseNum
    //     //     from verses
    //     //     """)).ToList();
    //     var allVerses = await _context.Verses.ToListAsync();
    //     var versesByKey = allVerses.ToDictionary(v => (v.Reference));
    //     _logger.LogDebug("Loaded {Count} verses", allVerses.Count);
    //
    //     SQLitePCL.Batteries.Init();
    //     using var connection = new SqliteConnection(@"Data Source=C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\CrossReferences\cross_references.db");
    //     await connection.OpenAsync();
    //
    //     using var cmd = connection.CreateCommand();
    //     cmd.CommandText =
    //         """
    //     create table if not exists cross_references (
    //         id integer primary key autoincrement,
    //         from_verse_id integer,
    //         to_passage_id integer,
    //         votes integer
    //     );
    //     create table if not exists cross_reference_passages (
    //         id integer primary key autoincrement,
    //         reference text
    //     );
    //     create table if not exists cross_reference_passages_verses (
    //         passage_id integer,
    //         verse_id integer
    //     );
    //     """;
    //     await cmd.ExecuteNonQueryAsync();
    //
    //     using var _cmd = connection.CreateCommand();
    //     _cmd.CommandText =
    //         """
    //         delete from cross_references;
    //         delete from cross_reference_passages;
    //         delete from cross_reference_passages_verses;
    //         """;
    //     await _cmd.ExecuteNonQueryAsync();
    //
    //     using var transaction = await connection.BeginTransactionAsync();
    //     cmd.Transaction = (SqliteTransaction)transaction;
    //
    //     try
    //     {
    //         //bool skip = true;
    //         foreach (string line in lines)
    //         {
    //             //if (line == "Gen.1.27\t1Cor.11.7-1Cor.11.9\t21") skip = false;
    //             //if (skip) continue;
    //
    //             string[] parts = line.Split('\t');
    //             if (parts[0] == "From Verse") continue;
    //
    //             Reference reference = ReferenceParser.Parse(parts[0]);
    //             Reference crossReference = ReferenceParser.Parse(parts[1]);
    //             int votes = int.Parse(parts[2]);
    //
    //             cmd.CommandText =
    //                 """
    //             insert into cross_reference_passages (reference)
    //             values ($Reference);
    //             select last_insert_rowid();
    //             """;
    //             cmd.Parameters.Clear();
    //             cmd.Parameters.AddWithValue("$Reference", crossReference.ReadableReference);
    //             long newPassageId = (long)(await cmd.ExecuteScalarAsync())!;
    //
    //             foreach (int verseNum in crossReference.VerseNumbers)
    //             {
    //                 if (!versesByKey.TryGetValue((crossReference.Book, crossReference.Chapter, verseNum), out var crossVerse))
    //                 {
    //                     _logger.LogInformation("Verse not found: {Book} {Chapter}:{Verse}", crossReference.Book, crossReference.Chapter, verseNum);
    //                     continue;
    //                 }
    //
    //                 cmd.CommandText =
    //                     """
    //                 insert into cross_reference_passages_verses (passage_id, verse_id)
    //                 values ($PassageId, $VerseId)
    //                 """;
    //                 cmd.Parameters.Clear();
    //                 cmd.Parameters.AddWithValue("$PassageId", newPassageId);
    //                 cmd.Parameters.AddWithValue("$VerseId", crossVerse.Id);
    //                 await cmd.ExecuteNonQueryAsync();
    //             }
    //
    //             if (!versesByKey.TryGetValue((reference.Book, reference.Chapter, reference.VerseNumbers.First()), out var fromVerse))
    //             {
    //                 _logger.LogInformation("From verse not found: {Book} {Chapter}:{Verse}", reference.Book, reference.Chapter, reference.VerseNumbers.First());
    //                 continue;
    //             }
    //
    //             cmd.CommandText =
    //                 """
    //             insert into cross_references (from_verse_id, to_passage_id, votes)
    //             values ($FromVerseId, $ToPassageId, $Votes)
    //             """;
    //             cmd.Parameters.Clear();
    //             cmd.Parameters.AddWithValue("$FromVerseId", fromVerse.Id);
    //             cmd.Parameters.AddWithValue("$ToPassageId", newPassageId);
    //             cmd.Parameters.AddWithValue("$Votes", votes);
    //             await cmd.ExecuteNonQueryAsync();
    //
    //             processed++;
    //             if (processed % 50 == 0 || processed == total)
    //             {
    //                 double percent = (double)processed / total * 100;
    //                 _logger.LogInformation("Progress: {Processed}/{Total} ({Percent:F1}%)", processed, total, percent);
    //             }
    //         }
    //
    //         await transaction.CommitAsync();
    //         _logger.LogInformation("Finished uploading cross references");
    //     }
    //     catch
    //     {
    //         await transaction.RollbackAsync();
    //         throw;
    //     }
    // }
}