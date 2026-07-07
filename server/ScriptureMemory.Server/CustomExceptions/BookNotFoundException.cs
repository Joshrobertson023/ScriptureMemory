using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.CustomExceptions;

public class BookNotFoundException : Exception
{
    public string Book { get; set; }
    
    public BookNotFoundException() { }
    public BookNotFoundException(string message) : base(message) { }
    public BookNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public BookNotFoundException(string message, string book)
        : base(message)
    {
        Book = book;
    }
}