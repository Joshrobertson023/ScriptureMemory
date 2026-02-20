using System;

namespace DataAccess.Models;

public class VerseOfDay
{
    public int Id { get; set; } = 0;
    public Reference Reference { get; set; }
    public int Sequence { get; set; } = 0;

    public VerseOfDay(Reference reference)
    {
        Reference = reference;
    }
}