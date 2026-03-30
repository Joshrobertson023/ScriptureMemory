//using System.Linq;
//using DataAccess.DataInterfaces;
//using DataAccess.DBAccess;
//using DataAccess.Models;

//namespace DataAccess.Data;

//public class BannerData : IBannerData
//{
//    private const int DefaultBannerId = 1;
//    private readonly IDBAccess _db;

//    public BannerData(IDBAccess db)
//    {
//        _db = db;
//    }

//    public async Task<SiteBanner?> GetBanner()
//    {
//        const string sql = @"
//SELECT
//    BANNER_ID AS BannerId,
//    MESSAGE AS Message
//FROM SITE_BANNER
//WHERE BANNER_ID = :bannerId";

//        var results = await _db.GetData<SiteBanner, dynamic>(sql, new { bannerId = DefaultBannerId }, "Default");
//        return results.FirstOrDefault();
//    }

//    public async Task SetBanner(SiteBanner banner)
//    {
//        const string sql = @"
//MERGE INTO SITE_BANNER target
//USING (SELECT :bannerId AS BANNER_ID FROM dual) source
//   ON (target.BANNER_ID = source.BANNER_ID)
// WHEN MATCHED THEN
//    UPDATE SET
//        MESSAGE = :message
// WHEN NOT MATCHED THEN
//    INSERT (BANNER_ID, MESSAGE)
//    VALUES (:bannerId, :message)";

//        await _db.SaveData<dynamic, dynamic>(
//            sql,
//            new
//            {
//                bannerId = DefaultBannerId,
//                message = banner.Message ?? string.Empty
//            },
//            "Default");
//    }

//    public async Task RemoveBanner()
//    {
//        const string sql = "DELETE FROM SITE_BANNER WHERE BANNER_ID = :bannerId";
//        await _db.SaveData<dynamic, dynamic>(sql, new { bannerId = DefaultBannerId }, "Default");
//    }
//}

