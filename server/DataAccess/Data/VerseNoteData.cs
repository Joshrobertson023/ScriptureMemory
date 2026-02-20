//using DataAccess.DBAccess;
//using DataAccess.Models;
//using DataAccess.Services;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using System.Collections.Generic;
//using System.Linq;
//using Dapper;
//using Oracle.ManagedDataAccess.Client;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;

//public class VerseNoteData : IVerseNoteData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;
//    private readonly INoteEncryptionService _encryptionService;

//    public VerseNoteData(IDBAccess db, IConfiguration config, INoteEncryptionService encryptionService)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
//        _encryptionService = encryptionService;
//    }

//    public async Task<VerseNote> InsertNote(VerseNote note)
//    {
//        var encryptedText = _encryptionService.Encrypt(note.Text);
//        var originalReference = note.OriginalReference ?? note.VerseReference;

//        var sql = @"INSERT INTO VERSE_NOTES (""VERSEREFERENCE"", ""USERNAME"", ""TEXT"", ""IS_PUBLIC"", ""APPROVED"", ""CREATEDDATE"", ORIGINAL_REFERENCE)
//                    VALUES (:VerseReference, :Username, :Text, :IsPublic, :Approved, SYSDATE, :OriginalReference)";
//        using IDbConnection conn = new OracleConnection(connectionString);
        
//        var parameters = new DynamicParameters();
//        parameters.Add(":VerseReference", note.VerseReference);
//        parameters.Add(":Username", note.Username);
//        parameters.Add(":Text", encryptedText);
//        parameters.Add(":IsPublic", note.IsPublic ? 1 : 0);
//        if (note.Approved.HasValue)
//            parameters.Add("Approved", 1);
//        else
//            parameters.Add("Approved", 0);
//        parameters.Add(":OriginalReference", originalReference);
        
//        await conn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
//        var getSql = @"SELECT * FROM (
//                           SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                                  ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                                  ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                           FROM VERSE_NOTES
//                           WHERE ""VERSEREFERENCE"" = :VerseReference 
//                           AND ""USERNAME"" = :Username 
//                           AND ""TEXT"" = :Text
//                           ORDER BY ""CREATEDDATE"" DESC
//                       ) WHERE ROWNUM = 1";
        
//        var insertedNote = await conn.QueryFirstOrDefaultAsync<VerseNote>(getSql, new
//        {
//            VerseReference = note.VerseReference,
//            Username = note.Username,
//            Text = encryptedText
//        }, commandType: CommandType.Text);
        
//        if (insertedNote != null)
//        {
//            insertedNote.Text = _encryptionService.Decrypt(insertedNote.Text);
//            return insertedNote;
//        }

//        return note;
//    }

//    public async Task<VerseNote> UpdateNote(VerseNote note)
//    {
//        var encryptedText = _encryptionService.Encrypt(note.Text);
        
//        var sql = @"UPDATE VERSE_NOTES 
//                    SET ""TEXT"" = :Text
//                    WHERE ""ID"" = :Id";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var parameters = new DynamicParameters();
//        parameters.Add(":Text", encryptedText);
//        parameters.Add(":Id", note.Id);
        
//        await conn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
        
//        // Return the updated note
//        var updatedNote = await GetNoteById(note.Id);
//        if (updatedNote == null)
//        {
//            throw new InvalidOperationException("Note not found after update");
//        }
//        return updatedNote;
//    }

//    public async Task DeleteNote(int id)
//    {
//        var sql = @"DELETE FROM VERSE_NOTES WHERE ""ID"" = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.Text);
//    }

//    public async Task<VerseNote?> GetNoteById(int id)
//    {
//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""ID"" = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<VerseNote>(sql, new { Id = id }, commandType: CommandType.Text);
        
//        if (result != null)
//        {
//            result.Text = _encryptionService.Decrypt(result.Text);
//        }
        
//        return result;
//    }

//    public async Task<List<VerseNote>> GetNotesByVerseReference(string verseReference, string? username = null)
//    {
//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""VERSEREFERENCE"" = :VerseReference";
        
//        if (!string.IsNullOrEmpty(username))
//        {
//            sql += " AND \"USERNAME\" = :Username";
//        }
        
//        sql += " ORDER BY \"CREATEDDATE\" DESC";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var parameters = new { VerseReference = verseReference, Username = username };
//        var results = await conn.QueryAsync<VerseNote>(sql, parameters, commandType: CommandType.Text);
        
//        return results.Select(note =>
//        {
//            note.Text = _encryptionService.Decrypt(note.Text);
//            return note;
//        }).ToList();
//    }

//    public async Task<List<VerseNote>> GetPublicNotesByVerseReference(string verseReference)
//    {
//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""VERSEREFERENCE"" = :VerseReference 
//                    AND ""IS_PUBLIC"" = 1 
//                    ORDER BY ""CREATEDDATE"" DESC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<VerseNote>(sql, new { VerseReference = verseReference }, commandType: CommandType.Text);
        
//        return results.Select(note => new VerseNote
//        {
//            Id = note.Id,
//            VerseReference = note.VerseReference,
//            Username = note.Username,
//            Text = _encryptionService.Decrypt(note.Text),
//            IsPublic = true,
//            Approved = note.Approved,
//            CreatedDate = note.CreatedDate,
//            OriginalReference = note.OriginalReference
//        }).ToList();
//    }

//    public async Task<List<VerseNote>> GetNotesByUsername(string username)
//    {
//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""USERNAME"" = :Username
//                    ORDER BY ""CREATEDDATE"" DESC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<VerseNote>(sql, new { Username = username }, commandType: CommandType.Text);
        
//        return results.Select(note =>
//        {
//            note.Text = _encryptionService.Decrypt(note.Text);
//            return note;
//        }).ToList();
//    }

//    public async Task<List<VerseNote>> GetUnapprovedNotes()
//    {
//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""IS_PUBLIC"" = 1 
//                    AND (""APPROVED"" = 0 OR ""APPROVED"" IS NULL)
//                    ORDER BY ""CREATEDDATE"" DESC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<VerseNote>(sql, commandType: CommandType.Text);
        
//        return results.Select(note => new VerseNote
//        {
//            Id = note.Id,
//            VerseReference = note.VerseReference,
//            Username = note.Username,
//            Text = _encryptionService.Decrypt(note.Text),
//            IsPublic = true,
//            Approved = false,
//            CreatedDate = note.CreatedDate,
//            OriginalReference = note.OriginalReference
//        }).ToList();
//    }

//    public async Task UpdateNoteApproval(int id, bool approved)
//    {
//        var sql = @"UPDATE VERSE_NOTES 
//                    SET ""APPROVED"" = :Approved 
//                    WHERE ""ID"" = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Id = id, Approved = approved ? 1 : 0 }, commandType: CommandType.Text);
//    }

//    public async Task DeleteNoteById(int id)
//    {
//        await DeleteNote(id);
//    }

//    public async Task<List<string>> GetVersesWithPrivateNotes(string username, string book, int chapter)
//    {
//        var verseReferences = new List<string>();
//        for (int verse = 1; verse <= 300; verse++)
//        {
//            verseReferences.Add($"{book} {chapter}:{verse}");
//        }

//        var sql = @"SELECT DISTINCT ""VERSEREFERENCE"" AS VerseReference
//                    FROM VERSE_NOTES
//                    WHERE ""USERNAME"" = :Username
//                    AND ""IS_PUBLIC"" = 0
//                    AND ""VERSEREFERENCE"" IN (";
        
//        var parameters = new DynamicParameters();
//        parameters.Add(":Username", username);
        
//        for (int i = 0; i < verseReferences.Count; i++)
//        {
//            if (i > 0) sql += ",";
//            sql += $":Ref{i}";
//            parameters.Add($":Ref{i}", verseReferences[i]);
//        }
        
//        sql += ")";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<string>(sql, parameters, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task<List<string>> GetVersesWithPublicNotes(string book, int chapter)
//    {
//        var verseReferences = new List<string>();
//        for (int verse = 1; verse <= 500; verse++)
//        {
//            verseReferences.Add($"{book} {chapter}:{verse}");
//        }

//        var sql = @"SELECT DISTINCT ""VERSEREFERENCE"" AS VerseReference
//                    FROM VERSE_NOTES
//                    WHERE ""IS_PUBLIC"" = 1
//                    AND ""VERSEREFERENCE"" IN (";
        
//        var parameters = new DynamicParameters();
        
//        for (int i = 0; i < verseReferences.Count; i++)
//        {
//            if (i > 0) sql += ",";
//            sql += $":Ref{i}";
//            parameters.Add($":Ref{i}", verseReferences[i]);
//        }
        
//        sql += ")";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<string>(sql, parameters, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task<List<VerseNote>> GetAllNotesByChapter(string book, int chapter, string? username = null, bool isPublic = true)
//    {
//        var verseReferences = new List<string>();
//        for (int verse = 1; verse <= 300; verse++)
//        {
//            verseReferences.Add($"{book} {chapter}:{verse}");
//        }

//        var sql = @"SELECT ""ID"" AS Id, ""VERSEREFERENCE"" AS VerseReference, ""USERNAME"" AS Username, 
//                           ""TEXT"" AS Text, ""IS_PUBLIC"" AS IsPublic, ""APPROVED"" AS Approved, 
//                           ""CREATEDDATE"" AS CreatedDate, ORIGINAL_REFERENCE AS OriginalReference
//                    FROM VERSE_NOTES
//                    WHERE ""IS_PUBLIC"" = :IsPublic";
        
//        if (isPublic)
//        {
//            sql += " AND (\"APPROVED\" = 1 OR \"APPROVED\" IS NULL)";
//        }
//        else
//        {
//            sql += " AND \"IS_PUBLIC\" = 0";
//            if (!string.IsNullOrEmpty(username))
//            {
//                sql += " AND \"USERNAME\" = :Username";
//            }
//        }
        
//        sql += " AND \"VERSEREFERENCE\" IN (";
        
//        var parameters = new DynamicParameters();
//        parameters.Add(":IsPublic", isPublic ? 1 : 0);
//        if (!isPublic && !string.IsNullOrEmpty(username))
//        {
//            parameters.Add(":Username", username);
//        }
        
//        for (int i = 0; i < verseReferences.Count; i++)
//        {
//            if (i > 0) sql += ",";
//            sql += $":Ref{i}";
//            parameters.Add($":Ref{i}", verseReferences[i]);
//        }
        
//        sql += ") ORDER BY \"CREATEDDATE\" DESC";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<VerseNote>(sql, parameters, commandType: CommandType.Text);
        
//        return results.Select(note =>
//        {
//            note.Text = _encryptionService.Decrypt(note.Text);
//            return note;
//        }).ToList();
//    }
//}

