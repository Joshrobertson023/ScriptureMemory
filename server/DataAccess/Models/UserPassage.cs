using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class UserPassage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CollectionId { get; set; }
    public Reference Reference { get; set; }
    public int OrderPosition { get; set; }
    public DateTime DateAdded { get; set; }
    public float ProgressPercent { get; set; } = 0.0f;
    public int TimesMemorized { get; set; } = 0;
    public List<Verse> Verses { get; set; } = new();
    public DateTime? LastPracticed { get; set; }
    public DateTime? DueDate { get; set; }

    public UserPassage() { }

    public UserPassage(int userId, int collectionId, Reference reference)
    {
        UserId = userId;
        CollectionId = collectionId;
        Reference = reference;
        DateAdded = DateTime.UtcNow;
    }

    public UserPassage(int userId, int collectionId, string readableReference)
    {
        UserId = userId;
        CollectionId = collectionId;
        Reference = new Reference(readableReference);
        DateAdded = DateTime.UtcNow;
    }
}
