using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class SearchData
{
    public List<Verse> Verses { get; set; } = new();
    public bool Searched_By_Passage { get; set; }
    public string Readable_Reference { get; set; }
}
