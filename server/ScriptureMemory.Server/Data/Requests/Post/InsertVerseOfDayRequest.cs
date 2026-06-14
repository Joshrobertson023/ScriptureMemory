using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public class InsertVerseOfDayRequest
{
    [Required] public string Reference { get; set; } = string.Empty;
    [Required] public int AdminId { get; set; }
}
