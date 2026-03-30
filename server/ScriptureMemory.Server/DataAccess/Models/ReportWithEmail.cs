namespace DataAccess.Models;

public class ReportWithEmail
{
    public int Report_Id { get; set; }
    public string Reporter_Username { get; set; }
    public string Reported_Username { get; set; }
    public string Reported_Email { get; set; }
    public string ReportReason { get; set; }
    public DateTime Created_Date { get; set; }
}


