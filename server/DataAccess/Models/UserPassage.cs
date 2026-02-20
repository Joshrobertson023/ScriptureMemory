using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class UserPassage
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int CollectionId { get; set; }
    public Reference Reference { get; set; }
    public DateTime LastPracticed { get; set; } = DateTime.UtcNow;
    public int OrderPosition { get; set; }
    public DateTime DateAdded { get; set; }
    public float ProgressPercent { get; set; } = 0.0f;
    public int TimesMemorized { get; set; } = 0;
    public List<Verse> Verses { get; set; } = new();
    public DateTime? DueDate { get; set; }

    public UserPassage(string username, int collectionId, Reference reference)
    {
        Username = username;
        CollectionId = collectionId;
        Reference = reference;
        DateAdded = DateTime.UtcNow;
    }
}
