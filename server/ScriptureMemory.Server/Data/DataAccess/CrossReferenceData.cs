// using Dapper;
// using DataAccess.Models;
// using Npgsql;
// using ScriptureMemory.Server.DataAccess.Models;
// using ScriptureMemory.Server.Tools;
//
// namespace ScriptureMemory.Server.DataAccess.Data;
//
// public class CrossReferenceData
// {
//     private readonly IConfiguration _config;
//     private readonly string _connectionString;
//
//     public CrossReferenceData(IConfiguration config)
//     {
//         _config = config;
//         _connectionString = _config.GetConnectionString("PostgresConnection")
//             ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
//     }
//
//     public class GetCrossReferenceDto
//     {
//         public string FromVerseBook { get; set; } = string.Empty;
//         public int FromChapter { get; set; }
//         public int FromVerse { get; set; }
//         public string FromText { get; set; } = string.Empty;
//         public int FromVerseId { get; set; }
//         public string CRPassageReference { get; set; } = string.Empty;
//         public string CRPassageBook { get; set; } = string.Empty;
//         public int CRPassageChapter { get; set; }
//         public int CRPassageVerse { get; set; }
//         public int CRVerseId { get; set; }
//         public string CRText { get; set; } = string.Empty;
//     }
//
//     public async Task<List<CrossReferenceResponse>> GetCrossReferences(List<int> verseIds)
//     {
//         using var conn = new NpgsqlConnection(_connectionString);
//
//         var results = await conn.QueryAsync<GetCrossReferenceDto>(
//              """
//             select
//             from_verse_book as FromVerseBook,
//             from_chapter as FromChapter,
//             from_verse as FromVerse,
//             from_text as FromText,
//             from_verse_id as FromVerseId,
//             crp_reference as CRPassageReference,
//             cr_v.book as CRPassageBook,
//             cr_v.chapter as CRPassageChapter,
//             cr_v.verse_num as CRPassageVerse,
//             cr_v.text as CRText,
//             cr_v.id as CRVerseId
//             from (
//               select
//               v.book as from_verse_book,
//               v.chapter as from_chapter,
//               v.verse_num as from_verse,
//               v.text as from_text,
//               v.id as from_verse_id,
//               crp.reference as CRP_Reference,
//               crpv.verse_id as crpv_verse_id
//               from cross_reference_passages crp
//               join cross_reference_passages_verses crpv on crp.id = crpv.passage_id
//               join cross_references cr on cr.to_passage_id = crpv.passage_id
//               join verses v on v.id = cr.from_verse_id
//               where v.id = ANY(@VerseIds) and cr.votes > 0
//               order by cr.votes desc
//             ) join verses cr_v on cr_v.id = crpv_verse_id
//             """, new { VerseIds = verseIds.ToArray() });
//
//         return results
//              .GroupBy(r => r.FromVerseId)
//              .Select(g =>
//              {
//                  return new CrossReferenceResponse
//                  {
//                      FromVerse = new Verse
//                      {
//                          Id = g.First().FromVerseId,
//                          Reference = ReferenceParser.Parse(
//                              g.First().FromVerseBook,
//                              g.First().FromChapter,
//                              new List<int> { g.First().FromVerse }),
//                          Text = g.First().FromText
//                      },
//                      CrossReferences = g
//                          .GroupBy(r => r.CRPassageReference)
//                          .Select(g => new Passage
//                          {
//                              Reference = ReferenceParser.Parse(
//                                  g.First().CRPassageBook,
//                                  g.First().CRPassageChapter,
//                                  g.Select(r => r.CRPassageVerse).ToList()),
//                              Verses = g.Select(r => new Verse
//                              {
//                                  Id = r.CRVerseId,
//                                  Reference = ReferenceParser.Parse(
//                                      r.CRPassageBook,
//                                      r.CRPassageChapter,
//                                      new List<int> { r.CRPassageVerse }),
//                                  Text = r.CRText
//                              }).ToList()
//                          }).ToList()
//                  };
//              }).ToList();
//     }
//
//     public async Task<List<CrossReferenceResponse>> GetCrossReferences(List<Reference> references)
//     {
//         using var conn = new NpgsqlConnection(_connectionString);
//
//         var results = await conn.QueryAsync<GetCrossReferenceDto>(
//             """
//             select
//             from_verse_book as FromVerseBook,
//             from_chapter as FromChapter,
//             from_verse as FromVerse,
//             from_text as FromText,
//             from_verse_id as FromVerseId,
//             crp_reference as CRPassageReference,
//             cr_v.book as CRPassageBook,
//             cr_v.chapter as CRPassageChapter,
//             cr_v.verse_num as CRPassageVerse,
//             cr_v.text as CRText,
//             cr_v.id as CRVerseId
//             from (
//               select
//               v.book as from_verse_book,
//               v.chapter as from_chapter,
//               v.verse_num as from_verse,
//               v.text as from_text,
//               v.id as from_verse_id,
//               crp.reference as CRP_Reference,
//               crpv.verse_id as crpv_verse_id
//               from cross_reference_passages crp
//               join cross_reference_passages_verses crpv on crp.id = crpv.passage_id
//               join cross_references cr on cr.to_passage_id = crpv.passage_id
//               join verses v on v.id = cr.from_verse_id
//               join unnest(@Books::text[], @Chapters::int[], @Verses::int[]) as ref(book, chapter, verse_num)
//                 on v.book = ref.book and v.chapter = ref.chapter and v.verse_num = ref.verse_num
//               where cr.votes > 0
//               order by cr.votes desc
//             ) join verses cr_v on cr_v.id = crpv_verse_id
//             """, new 
//             { 
//                 Books = references.Select(r => r.Book.Trim()).ToArray(), 
//                 Chapters = references.Select(r => r.Chapter).ToArray(), 
//                 Verses = references.Select(r => r.VerseNumbers.First()).ToArray()
//             });
//
//         return results
//              .GroupBy(r => r.FromVerseId)
//              .Select(g =>
//              {
//                  return new CrossReferenceResponse
//                  {
//                      FromVerse = new Verse
//                      {
//                          Id = g.First().FromVerseId,
//                          Reference = ReferenceParser.Parse(
//                              g.First().FromVerseBook,
//                              g.First().FromChapter,
//                              new List<int> { g.First().FromVerse }),
//                          Text = g.First().FromText
//                      },
//                      CrossReferences = g
//                          .GroupBy(r => r.CRPassageReference)
//                          .Select(g => new Passage
//                          {
//                              Reference = ReferenceParser.Parse(
//                                  g.First().CRPassageBook,
//                                  g.First().CRPassageChapter,
//                                  g.Select(r => r.CRPassageVerse).ToList()),
//                              Verses = g.Select(r => new Verse
//                              {
//                                  Id = r.CRVerseId,
//                                  Reference = ReferenceParser.Parse(
//                                      r.CRPassageBook,
//                                      r.CRPassageChapter,
//                                      new List<int> { r.CRPassageVerse }),
//                                  Text = r.CRText
//                              }).ToList()
//                          }).ToList()
//                  };
//              }).ToList();
//     }
// }
