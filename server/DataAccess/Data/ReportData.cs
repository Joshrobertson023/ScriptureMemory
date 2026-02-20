using DataAccess.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
using DataAccess.DataInterfaces;

namespace DataAccess.Data;

public class ReportData : IReportData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public ReportData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default");
    }

    public async Task CreateReport(Report report)
    {
        var sql = @"INSERT INTO REPORTS (REPORTER_USERNAME, REPORTED_USERNAME, REASON, STATUS)
                    VALUES (:ReporterUsername, :ReportedUsername, :Reason, :Status)";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            ReporterUsername = report.Reporter_Username,
            ReportedUsername = report.Reported_Username,
            Reason = report.Reason,
            Status = string.IsNullOrWhiteSpace(report.Status) ? "OPEN" : report.Status
        }, commandType: CommandType.Text);
    }

    public async Task<IEnumerable<Report>> GetAllReports()
    {
        var sql = @"SELECT REPORT_ID as Report_Id,
                           REPORTER_USERNAME as Reporter_Username,
                           REPORTED_USERNAME as Reported_Username,
                           REASON,
                           CREATED_DATE as Created_Date,
                           STATUS
                    FROM REPORTS
                    ORDER BY CREATED_DATE DESC";

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<Report>(sql, commandType: CommandType.Text);
        return results;
    }

    public async Task<IEnumerable<ReportWithEmail>> GetAllReportsWithEmail()
    {
        var sql = @"SELECT r.REPORT_ID as Report_Id,
                           r.REPORTER_USERNAME as Reporter_Username,
                           r.REPORTED_USERNAME as Reported_Username,
                           r.REASON as ReportReason,
                           r.CREATED_DATE as Created_Date
                    FROM REPORTS r
                    ORDER BY r.CREATED_DATE DESC";

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<ReportWithEmail>(sql, commandType: CommandType.Text);
        
        
        var userReports = results.Where(r => r.Reported_Username != "SYSTEM").ToList();
        if (userReports.Any())
        {
            var usernames = userReports.Select(r => r.Reported_Username).Distinct().ToList();
            var emailSql = @"SELECT USERNAME, EMAIL FROM USERS WHERE USERNAME IN (:Usernames)";
            var users = await conn.QueryAsync<dynamic>(emailSql, new { Usernames = usernames }, commandType: CommandType.Text);
            var emailDict = users.ToDictionary(u => (string)u.USERNAME, u => (string)u.EMAIL);
            
            foreach (var report in userReports)
            {
                report.Reported_Email = emailDict.ContainsKey(report.Reported_Username) 
                    ? emailDict[report.Reported_Username] 
                    : "";
            }
        }
        
        
        foreach (var report in results.Where(r => r.Reported_Username == "SYSTEM"))
        {
            report.Reported_Email = report.ReportReason ?? "";
        }
        
        return results;
    }

    public async Task DeleteReport(int reportId)
    {
        var sql = @"DELETE FROM REPORTS WHERE REPORT_ID = :ReportId";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { ReportId = reportId }, commandType: CommandType.Text);
    }
}


