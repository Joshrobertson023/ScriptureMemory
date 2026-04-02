using CsvHelper;
using Dapper;
using DataAccess.Data;
using DataAccess.Models;
using Npgsql;
using ScriptureMemory.Server.Tools;
using System.Data;
using System.Globalization;

namespace ScriptureMemory.Server.Tools;

public sealed class VerseManagement
{
    private readonly VerseData _verseData;
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly ILogger<VerseManagement> _logger;

    public VerseManagement(
        VerseData verseData,
        IConfiguration config,
        ILogger<VerseManagement> logger)
    {
        _verseData = verseData;
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
        _logger = logger;
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
                allVerses.Add(new Verse
                {
                    Id = record.VERSE_ID,
                    Reference = ReferenceParser.ConvertStringToReference(record.VERSE_REFERENCE),
                    Text = record.VERSE_TEXT,
                    UsersSavedCount = record.USERS_SAVED_VERSE,
                    UsersMemorizedCount = record.USERS_MEMORIZED
                });
            }
        }

        List<Files.CsvRecordModels.Verse> csvVerses = new();

        foreach (var verse in allVerses)
        {
            string reference = verse.Reference.ToString();

            csvVerses.Add(new Files.CsvRecordModels.Verse
            {
                Id = verse.Id,
                Book = ReferenceParser.GetBook(reference),
                Chapter = ReferenceParser.GetChapter(reference),
                VerseNum = ReferenceParser.GetIndividualVerses(reference).FirstOrDefault(),
                Text = CleanVerses.CleanVerse(verse).Text
            });
        }

        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(csvVerses);
        }

        _logger.LogDebug("Finished moving verses");
    }

    public async Task UploadVersesToPostgres()
    {
        using var reader = new StreamReader(@"C:\Users\there\ScriptureMemory\server\ScriptureMemory.Server\Files\Verses\Kjv.csv");

        List<Files.CsvRecordModels.Verse> allVerses = new();
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Files.CsvRecordModels.Verse>();
            foreach (var record in records)
            {
                allVerses.Add(new Files.CsvRecordModels.Verse
                {
                    Book = record.Book,
                    Chapter = record.Chapter,
                    VerseNum = record.VerseNum,
                    Text = record.Text
                });
            }
        }

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(
            """
            insert into verses (book, chapter, text, memorized_count, saved_count, verse_num)
            values (@Book, @Chapter, @Text, 0, 0, @VerseNum)
            """,
            allVerses);

        _logger.LogDebug("Finished uploading verses to postgres");
    }
}
