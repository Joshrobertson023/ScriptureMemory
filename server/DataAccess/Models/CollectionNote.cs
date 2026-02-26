using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class CollectionNote
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int OrderPosition { get; set; }
}

