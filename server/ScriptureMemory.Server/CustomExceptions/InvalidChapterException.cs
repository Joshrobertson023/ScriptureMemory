namespace ScriptureMemory.Server.CustomExceptions;

public class InvalidChapterException : System.Exception
{
    public int Chapter { get; set; }
    public string Book { get; set; }
    
    public InvalidChapterException() { }
    public InvalidChapterException(string message) : base(message) { }
    public InvalidChapterException(string message, System.Exception innerException)
        : base(message, innerException) { }

    public InvalidChapterException(string message, int chapter, string book)
        : base(message)
    {
        Chapter = chapter;
        Book = book;
    }
}