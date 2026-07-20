namespace ScriptureMemory.Server.CustomExceptions;

public class BibleUnavailableException : System.Exception
{
    public string Bible { get; set; }
    
    public BibleUnavailableException() { }
    public BibleUnavailableException(string message) : base(message) { }

    public BibleUnavailableException(string message, System.Exception innerException)
        : base(message, innerException) { }

    public BibleUnavailableException(string message, string bible)
        : base(message)
    {
        Bible = bible;
    }
}