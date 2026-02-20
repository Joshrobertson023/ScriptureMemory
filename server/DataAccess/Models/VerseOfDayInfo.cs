using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class VerseOfDayInfo
{
    public DateTime LastUsedVodUtc { get; set; }
    public int LastUsedVodId { get; set; }
}
