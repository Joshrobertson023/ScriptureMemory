using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Models;

public class Relationship
{
    public string Username1 { get; set; }
    public string Username2 { get; set; }
    public RelationshipStatus Status { get; set; } 

    public Relationship(string username1, string username2)
    {
        Username1 = username1;
        Username2 = username2;
        Status = RelationshipStatus.Pending;
    }
}
