using System;

namespace DataAccess.Models;
public class PracticeLog
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime PracticeDate { get; set; }
    public DateTime LastUpdated { get; set; }
}



