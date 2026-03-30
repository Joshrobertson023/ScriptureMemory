namespace DataAccess.Models;

public class Report
{
    public int Report_Id { get; set; }
    public string Reporter_Username { get; set; }
    public string Reported_Username { get; set; }
    public string Reason { get; set; }
    public DateTime Created_Date { get; set; }
    public string Status { get; set; }
}


