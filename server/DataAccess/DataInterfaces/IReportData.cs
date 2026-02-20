using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IReportData
{
    Task CreateReport(Report report);
    Task<IEnumerable<Report>> GetAllReports();
    Task<IEnumerable<ReportWithEmail>> GetAllReportsWithEmail();
    Task DeleteReport(int reportId);
}


