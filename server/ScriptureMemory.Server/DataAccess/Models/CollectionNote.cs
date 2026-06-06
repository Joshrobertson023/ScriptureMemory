using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class CollectionNote
{
    [Key]
    public int Id { get; set; }

    public Collection CollectionNavigation { get; set; } = null!;
    
    public string Text { get; set; } = string.Empty;
    public int OrderPosition { get; set; }
}

