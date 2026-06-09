using ScriptureMemory.Server.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class UserPassage : Passage
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public Collection CollectionNavigation { get; set; } = null!;
    public int OrderPosition { get; set; }
    public DateTime DateAdded { get; set; }
    public float ProgressPercent { get; set; } = 0.0f;
    public int TimesMemorized { get; set; } = 0;
    public DateTime? LastPracticed { get; set; }
    public DateTime? DueDate { get; set; }
    public bool NotifyMemorized { get; set; } = true;
}
