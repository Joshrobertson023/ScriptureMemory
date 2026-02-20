using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public class PracticeSession
{
    public int SessionId { get; set; }
    public string Username { get; set; }
    public int? UserVerseId { get; set; }
    public string ReadableReference { get; set; }
    public int PracticeStyle { get; set; } // Enum: PracticeStyle
    public decimal AccuracyPercent { get; set; }
    public int StageCount { get; set; }
    public string StageAccuracies { get; set; } // JSON array of accuracies per stage
    public DateTime PracticeDate { get; set; }
    public DateTime CreatedDate { get; set; }
}





















