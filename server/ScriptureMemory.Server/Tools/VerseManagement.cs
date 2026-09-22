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
using ScriptureMemory.Server.Data.Models;
using Pgvector.EntityFrameworkCore;

namespace ScriptureMemory.Server.Tools;

public sealed class VerseManagement
{
    private readonly VerseDataEfCore _verseData;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly ILogger<VerseManagement> _logger;
    private readonly EmbeddingGenerator _embeddingGenerator;
    private readonly NpgsqlDataSource _dataSource;

    public VerseManagement(
        VerseDataEfCore verseData,
        ApplicationDbContext context,
        IConfiguration config,
        ILogger<VerseManagement> logger,
        EmbeddingGenerator embeddingGenerator,
        NpgsqlDataSource dataSource)
    {
        _verseData = verseData;
        _context = context;
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
        _logger = logger;
        _embeddingGenerator = embeddingGenerator;
        _dataSource = dataSource;
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

        List<Files.CsvRecordModels.CsvRecordVerse> csvVerses = new();

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

    private List<Verse> CreateVersesAndRemoveDuplicates(List<ScriptureMemory.Server.Files.CsvRecordModels.CsvRecordVerse> verses)
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
                var verses = CreateVersesAndRemoveDuplicates(csv.GetRecords<Files.CsvRecordModels.CsvRecordVerse>().ToList());
                
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
                    
                    await _context.Verses.AddRangeAsync(versesInBatch);
                    await _context.SaveChangesAsync();
                    
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

    public async Task UploadCrossReferences()
    {
        string[] lines = File.ReadAllLines(@"C:\Users\Josh Robertson\RiderProjects\ScriptureMemory\server\ScriptureMemory.Server\Files\CrossReferences\cross_references.txt");
        int total = lines.Length;
        int processed = 0;

        _logger.LogInformation("Loading all verses into memory...");
        var allVerses = await _context.Verses.ToListAsync();
        _logger.LogInformation("Loaded {Count} verses", allVerses.Count);

        List<string> passagesWithoutBook = new();
        List<string> versesWithoutToPassageReference = new();
        HashSet<string> uniqueBooks = new();
        Dictionary<string, Passage> passagesAdded = new();

        try
        {
            //bool skip = true;
            foreach (string line in lines)
            {
                //if (line == "Gen.1.27\t1Cor.11.7-1Cor.11.9\t21") skip = false;
                //if (skip) continue;

                string[] parts = line.Split('\t');
                if (parts[0] == "From Verse") continue;

                Reference fromVerseReference = ReferenceParser.Parse(parts[0]);

                string book = "";
                string parts1 = parts[1].ToLower();
                for (int i = 0; i < parts1.Length; i++)
                {
                    if (parts1[i] == '.')
                        break;
                    book += parts1[i];
                    if (book == "Ps") book = "psa";
                    else if (book == "Prov") book = "pro";
                    else if (book == "1john") book = "1jn";
                    else if (book == "acts") book = "act";
                    else if (book == "heb") book = "heb";
                    else if (book == "eph") book = "eph";
                    else if (book == "exod") book = "exo";
                    else if (book == "neh") book = "neh";
                    else if (book == "rev") book = "rev";
                    else if (book == "eccl") book = "ecc";
                    else if (book == "prov") book = "pro";
                    else if (book == "jer") book = "jer";
                    else if (book == "ps") book = "psa";
                    else if (book == "isa") book = "isa";
                    else if (book == "john") book = "jhn";
                    else if (book == "job") book = "job";
                    else if (book == "2pet") book = "2pe";
                    else if (book == "zech") book = "zec";
                    else if (book == "mark") book = "mrk";
                    else if (book == "col") book = "col";
                    else if (book == "matt") book = "mat";
                    else if (book == "1cor") book = "1co";
                    else if (book == "rom") book = "rom";
                    else if (book == "1chr") book = "1ch";
                    else if (book == "nah") book = "nam";
                    else if (book == "2cor") book = "2co";
                    else if (book == "1tim") book = "1ti";
                    else if (book == "gen") book = "gen";
                    else if (book == "1thess") book = "1th";
                    else if (book == "jonah") book = "jon";
                    else if (book == "deut") book = "deu";
                    else if (book == "luke") book = "luk";
                    else if (book == "jas") book = "jas";
                    else if (book == "gal") book = "gal";
                    else if (book == "amos") book = "amo";
                    else if (book == "joel") book = "jol";
                    else if (book == "ezek") book = "ezk";
                    else if (book == "josh") book = "jos";
                    else if (book == "hab") book = "hab";
                    else if (book == "1kgs") book = "1ki";
                    else if (book == "lev") book = "lev";
                    else if (book == "mal") book = "mal";
                    else if (book == "hos") book = "hos";
                    else if (book == "lam") book = "lam";
                    else if (book == "2chr") book = "2ch";
                    else if (book == "2kgs") book = "2ki";
                    else if (book == "num") book = "num";
                    else if (book == "1sam") book = "1sa";
                    else if (book == "dan") book = "dan";
                    else if (book == "1pet") book = "1pe";
                    else if (book == "ruth") book = "rut";
                    else if (book == "2sam") book = "2sa";
                    else if (book == "judg") book = "jdg";
                    else if (book == "2thess") book = "2th";
                    else if (book == "mic") book = "mic";
                    else if (book == "titus") book = "tit";
                    else if (book == "esth") book = "est";
                    else if (book == "jude") book = "jud";
                    else if (book == "zeph") book = "zep";
                    else if (book == "song") book = "sng";
                    else if (book == "ezra") book = "ezr";
                    else if (book == "2tim") book = "2ti";
                    else if (book == "phil") book = "php";
                    else if (book == "obad") book = "oba";
                    else if (book == "hag") book = "hag";
                    else if (book == "phlm") book = "phm";
                    else if (book == "3john") book = "3jn";
                    else if (book == "2john") book = "2jn";
                }

                Reference toPassageReference;
                if (parts1.Contains('-'))
                {
                    string[] rangeParts = parts1.Split('-');

                    int firstDot = rangeParts[0].IndexOf('.');
                    string[] firstSegments = rangeParts[0].Substring(firstDot + 1).Split('.');
                    int chapter1 = int.Parse(firstSegments[0]);
                    int verse1 = int.Parse(firstSegments[1]);

                    int secondDot = rangeParts[1].IndexOf('.');
                    string rawBook2 = rangeParts[1].Substring(0, secondDot);
                    string[] secondSegments = rangeParts[1].Substring(secondDot + 1).Split('.');
                    int chapter2 = int.Parse(secondSegments[0]);
                    int verse2 = int.Parse(secondSegments[1]);

                    if (book == rawBook2 && chapter1 == chapter2)
                    {
                        var verseNumbers = Enumerable.Range(verse1, verse2 - verse1 + 1).ToList();
                        toPassageReference = new Reference(new Book(book), chapter1, verseNumbers);
                    }
                    else
                    {
                        _logger.LogWarning("Cross-chapter/book range hit fallback parser: {Line}", parts1);
                        toPassageReference = ReferenceParser.Parse(
                            book + rangeParts[0].Substring(firstDot) + "-" + book + rangeParts[1].Substring(secondDot));
                    }
                }
                else
                {
                    int dotIndex = parts1.IndexOf('.');
                    string versesSegment = parts1.Substring(dotIndex + 1);
                    string[] chapterAndVerses = versesSegment.Split('.');
                    int chapter = int.Parse(chapterAndVerses[0]);
                    var verseNumbers = chapterAndVerses[1]
                        .Split(',')
                        .Select(v => int.Parse(v.Trim()))
                        .ToList();

                    toPassageReference = new Reference(new Book(book), chapter, verseNumbers);
                }

                List<Verse> versesInPassage = new();
                if (toPassageReference == null)
                {
                    _logger.LogInformation("toPassageReference was null: " + parts[1]);
                }
                else
                {
                    versesWithoutToPassageReference.Add(parts[1]);
                    versesInPassage = allVerses.Where(v => toPassageReference.VerseIds.Contains(v.Id)).ToList();
                    int votes = int.Parse(parts[2]);
                    if (toPassageReference.Book == null)
                    {
                        _logger.LogInformation("toPassageReference.Book was null.");
                        passagesWithoutBook.Add(toPassageReference.ReadableReference);
                    }
                    else
                    {
                        _ = toPassageReference.ReadableReference;
                        _ = fromVerseReference.ReadableReference;
                        var toPassage = new Passage
                        {
                            Reference = toPassageReference,
                            Verses = versesInPassage
                        };
                        if (!passagesAdded.TryGetValue(toPassage.Id, out var existingPassage))
                        {
                            _context.Passages.Add(toPassage);
                            passagesAdded[toPassage.Id] = toPassage;
                        }
                        else
                        {
                            toPassage = existingPassage;
                        }
                        var newCrossReference = new CrossReference()
                        {
                            FromVerse = allVerses.Single(v => v.Id == fromVerseReference.VerseId),
                            ToPassage = toPassage,
                            Votes = votes
                        };
                        _context.CrossReferences.Add(newCrossReference);
                    }
                }

                processed++;
                if (processed % 50 == 0 || processed == total)
                {
                    await _context.SaveChangesAsync();

                    double percent = (double)processed / total * 100;
                    _logger.LogInformation("Progress: {Processed}/{Total} ({Percent:F1}%)", processed, total, percent);
                }
            }

            _logger.LogInformation("Finished uploading cross references");
            _logger.LogInformation("Passages not added: ");
            foreach (var passage in passagesWithoutBook)
            {
                _logger.LogInformation($"{passage}");
            }
            _logger.LogInformation("Verses not added: ");
            foreach (var passage in versesWithoutToPassageReference)
            {
                _logger.LogInformation($"{passage}");
            }
        }
        catch
        {
            throw;
        }
    }
}