using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgvector;
using ScriptureMemory.Server.Tools;
using static DataAccess.Data.VerseData;

namespace DataAccess.Models;


public class Verse
{
    public string Id { get; set; } = string.Empty;
    [MaxLength(30)]
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int VerseNum { get; set; }
    public int MemorizedCount { get; set; }
    public int SavedCount { get; set; }
}
