namespace DataAccess.Models;

public class SiteBanner
{
    public int Id { get; set; } = 0;
    public string Message { get; set; }

    public SiteBanner(string message)
    {
        Message = message;
    }
}

