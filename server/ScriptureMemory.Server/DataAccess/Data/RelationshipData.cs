//using DataAccess.Models;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;

//public class RelationshipData : IRelationshipData
//{
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public RelationshipData(IConfiguration config)
//    {
//        _config = config;
//        connectionString = _config.GetConnectionString("Default");
//    }

//    public async Task CreateRelationship(Relationship relationship)
//    {
//        var sql = @"INSERT INTO USER_RELATIONSHIPS (USERNAME_1, USERNAME_2, TYPE)
//                    VALUES (:Username1, :Username2, :Type)";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Username1 = relationship.Username1,
//            Username2 = relationship.Username2,
//            Type = relationship.Type
//        }, commandType: CommandType.Text);
//    }

//    public async Task UpdateRelationship(string username1, string username2, int type)
//    {
//        var sql = @"UPDATE USER_RELATIONSHIPS SET TYPE = :Type 
//                    WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) 
//                    OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Username1 = username1,
//            Username2 = username2,
//            Type = type
//        }, commandType: CommandType.Text);
//    }

//    public async Task<Relationship?> GetRelationship(string username1, string username2)
//    {
//        var sql = @"SELECT * FROM USER_RELATIONSHIPS 
//                    WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) 
//                    OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<Relationship>(sql, new
//        {
//            Username1 = username1,
//            Username2 = username2
//        }, commandType: CommandType.Text);
        
//        return result;
//    }

//    public async Task<List<User>> GetFriends(string username)
//    {
//        var sql = @"SELECT f.FRIEND_USERNAME AS Username, u.FIRSTNAME AS FirstName, u.LASTNAME AS LastName, u.PROFILE_PICTURE_URL AS ProfilePictureUrl
//                    FROM (
//                        SELECT DISTINCT 
//                        CASE 
//                            WHEN USERNAME_1 = :Username THEN USERNAME_2 
//                            ELSE USERNAME_1 
//                        END AS FRIEND_USERNAME
//                        FROM USER_RELATIONSHIPS
//                        WHERE ((USERNAME_1 = :Username OR USERNAME_2 = :Username) AND TYPE = 1)
//                    ) f
//                    JOIN USERS u ON u.USERNAME = f.FRIEND_USERNAME";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryAsync<User>(sql, new { Username = username }, commandType: CommandType.Text);
//        return result.ToList();
//    }

//    public async Task<List<User>> GetFriendNames(string username)
//    {
//        var sql = @"SELECT f.FRIEND_USERNAME AS Username, u.FIRSTNAME AS FirstName, u.LASTNAME AS LastName
//                    FROM (
//                        SELECT DISTINCT 
//                        CASE 
//                            WHEN USERNAME_1 = :Username THEN USERNAME_2 
//                            ELSE USERNAME_1 
//                        END AS FRIEND_USERNAME
//                        FROM USER_RELATIONSHIPS
//                        WHERE ((USERNAME_1 = :Username OR USERNAME_2 = :Username) AND TYPE = 1)
//                    ) f
//                    JOIN USERS u ON u.USERNAME = f.FRIEND_USERNAME";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryAsync<User>(sql, new { Username = username }, commandType: CommandType.Text);
//        return result.ToList();
//    }

//    public async Task DeleteRelationship(string username1, string username2)
//    {
//        var sql = @"DELETE FROM USER_RELATIONSHIPS 
//                    WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) 
//                    OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Username1 = username1,
//            Username2 = username2
//        }, commandType: CommandType.Text);
//    }

//    public async Task<List<string>> GetPendingRequests(string username)
//    {
//        var sql = @"SELECT USERNAME_2 FROM USER_RELATIONSHIPS WHERE USERNAME_1 = :Username AND TYPE = 0";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryAsync<string>(sql, new { Username = username }, commandType: CommandType.Text);
//        return result.ToList();
//    }

//    public async Task<bool> AreFriends(string username1, string username2)
//    {
//        var relationship = await GetRelationship(username1, username2);
//        return (relationship != null && relationship.Type == 1);
//    }
//}

