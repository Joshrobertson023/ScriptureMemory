using ScriptureMemory.Server.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;

[NotMapped]
public class UserPassage : Passage
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public User User { get; set; } = new();
    
    public int CollectionId { get; set; }
    
    public Collection CollectionNavigation { get; set; } = null!;
    
    public int OrderPosition { get; set; } // The order the passage is in the collection
    
    public DateTime DateAdded { get; set; }
    
    public float ProgressPercent { get; set; }
    
    public int TimesMemorized { get; set; }
    
    public DateTime? LastPracticed { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool NotifyMemorized { get; set; }
}
