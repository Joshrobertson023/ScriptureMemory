//using DataAccess.DBAccess;
//using DataAccess.Models;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System.Data;
//using System.Collections.Generic;
//using System.Linq;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;

//public class CategoryData : ICategoryData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public CategoryData(IDBAccess db, IConfiguration config)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
//    }

//    public async Task<IEnumerable<Category>> GetAll()
//    {
//        const string sql = @"SELECT CATEGORY_ID as CategoryId, NAME FROM CATEGORIES ORDER BY NAME";
//        return await _db.GetData<Category, dynamic>(sql, new { }, "Default");
//    }

//    public async Task<Category?> GetCategoryByName(string name)
//    {
//        const string sql = @"SELECT CATEGORY_ID as CategoryId, NAME FROM CATEGORIES WHERE UPPER(NAME) = UPPER(:name) FETCH FIRST 1 ROW ONLY";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<Category>(sql, new { name }, commandType: CommandType.Text);
//        return result;
//    }

//    public async Task<IEnumerable<Category>> GetTop(int limit)
//    {
//        // First try to get top categories by number of published collections
//        const string sql = @"
//            SELECT c.CATEGORY_ID as CategoryId, c.NAME as Name
//            FROM CATEGORIES c
//            LEFT JOIN COLLECTION_CATEGORIES cc ON c.CATEGORY_ID = cc.CATEGORY_ID
//            LEFT JOIN PUBLISHED_COLLECTIONS pc ON cc.COLLECTION_ID = pc.COLLECTION_ID
//            WHERE pc.COLLECTION_ID IS NOT NULL
//            GROUP BY c.CATEGORY_ID, c.NAME
//            ORDER BY COUNT(pc.COLLECTION_ID) DESC, c.NAME
//            FETCH FIRST :Limit ROWS ONLY";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<Category>(sql, new { Limit = limit }, commandType: CommandType.Text);
//        var topCategories = results.ToList();
        
//        // If we got fewer results than the limit, fill with all categories
//        if (topCategories.Count < limit)
//        {
//            const string allSql = @"SELECT CATEGORY_ID as CategoryId, NAME FROM CATEGORIES ORDER BY NAME";
//            var allCategories = await conn.QueryAsync<Category>(allSql, commandType: CommandType.Text);
//            var allList = allCategories.ToList();
            
//            // Add categories that aren't already in topCategories
//            var existingIds = new HashSet<int>(topCategories.Select(c => c.CategoryId));
//            foreach (var cat in allList)
//            {
//                if (!existingIds.Contains(cat.CategoryId) && topCategories.Count < limit)
//                {
//                    topCategories.Add(cat);
//                }
//            }
//        }
        
//        return topCategories;
//    }

//    public async Task<IEnumerable<int>> GetCategoryIdsForCollection(int collectionId)
//    {
//        const string sql = @"SELECT CATEGORY_ID FROM COLLECTION_CATEGORIES WHERE COLLECTION_ID = :collectionId";
//        var rows = await _db.GetData<dynamic, dynamic>(sql, new { collectionId }, "Default");
//        return rows.Select(r => (int)r.CATEGORY_ID);
//    }

//    public async Task<IEnumerable<int>> GetCollectionIdsForCategory(int categoryId)
//    {
//        const string sql = @"SELECT COLLECTION_ID FROM COLLECTION_CATEGORIES WHERE CATEGORY_ID = :categoryId";
//        var rows = await _db.GetData<dynamic, dynamic>(sql, new { categoryId }, "Default");
//        return rows.Select(r => (int)r.COLLECTION_ID);
//    }

//    public async Task SetCategoriesForCollection(int collectionId, IEnumerable<int> categoryIds)
//    {
//        const string deleteSql = @"DELETE FROM COLLECTION_CATEGORIES WHERE COLLECTION_ID = :collectionId";
//        await _db.SaveData<dynamic, dynamic>(deleteSql, new { collectionId }, "Default");

//        foreach (var categoryId in categoryIds.Distinct())
//        {
//            const string insertSql = @"INSERT INTO COLLECTION_CATEGORIES (COLLECTION_ID, CATEGORY_ID) VALUES (:collectionId, :categoryId)";
//            await _db.SaveData<dynamic, dynamic>(insertSql, new { collectionId, categoryId }, "Default");
//        }
//    }

//    public async Task Create(string name)
//    {
//        const string insertSql = @"INSERT INTO CATEGORIES (NAME) VALUES (:name)";
//        await _db.SaveData<dynamic, dynamic>(insertSql, new { name }, "Default");
//    }

//    public async Task RemoveCategoryLinks(int categoryId)
//    {
//        const string deleteLinksSql = @"DELETE FROM COLLECTION_CATEGORIES WHERE CATEGORY_ID = :categoryId";
//        await _db.SaveData<dynamic, dynamic>(deleteLinksSql, new { categoryId }, "Default");
//    }

//    public async Task Delete(int categoryId)
//    {
//        await RemoveCategoryLinks(categoryId);
//        // Also delete verse category links
//        const string deleteVerseLinksSql = @"DELETE FROM VERSE_CATEGORIES WHERE CATEGORY_ID = :categoryId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(deleteVerseLinksSql, new { categoryId }, commandType: CommandType.Text);
        
//        const string deleteCategorySql = @"DELETE FROM CATEGORIES WHERE CATEGORY_ID = :categoryId";
//        await _db.SaveData<dynamic, dynamic>(deleteCategorySql, new { categoryId }, "Default");
//    }

//    public async Task<List<string>> GetVersesInCategory(int categoryId)
//    {
//        const string sql = @"SELECT VERSE_REFERENCE FROM VERSE_CATEGORIES WHERE CATEGORY_ID = :categoryId ORDER BY VERSE_REFERENCE";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<string>(sql, new { categoryId }, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task AddVerseToCategory(int categoryId, string verseReference)
//    {
//        // Check if already exists
//        const string checkSql = @"SELECT COUNT(*) FROM VERSE_CATEGORIES WHERE CATEGORY_ID = :categoryId AND VERSE_REFERENCE = :verseReference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var count = await conn.ExecuteScalarAsync<int>(checkSql, new { categoryId, verseReference }, commandType: CommandType.Text);
        
//        if (count == 0)
//        {
//            const string insertSql = @"INSERT INTO VERSE_CATEGORIES (CATEGORY_ID, VERSE_REFERENCE) VALUES (:categoryId, :verseReference)";
//            await conn.ExecuteAsync(insertSql, new { categoryId, verseReference }, commandType: CommandType.Text);
//        }
//    }

//    public async Task DeleteVerseFromCategory(int categoryId, string verseReference)
//    {
//        const string deleteSql = @"DELETE FROM VERSE_CATEGORIES WHERE CATEGORY_ID = :categoryId AND VERSE_REFERENCE = :verseReference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(deleteSql, new { categoryId, verseReference }, commandType: CommandType.Text);
//    }
//}


