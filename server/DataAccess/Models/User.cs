using static VerseAppLibrary.Enums;

namespace DataAccess.Models;

public class User
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? HashedPassword { get; set; }
    public string? AuthToken { get; set; }
    public Status Status { get; set; } = Status.Active;
    public DateTime? DateRegistered { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public UserSettings Settings { get; set; } = new();
    public string? ProfileDescription { get; set; }
    public string? PushNotificationToken { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int VersesMemorizedCount { get; set; } = 0;
    public bool IsAdmin { get; set; } = false;

    public int Points { get; set; } = 0;

    public Paid? Paid { get; set; }
    public byte CollectionsCount { get; set; } = 0;

    public User( // Minimum to retain user identity
        string username,
        string firstName,
        string lastName,
        string email)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public User( // Create a new user
        string username,
        string firstName,
        string lastName,
        string email,
        string hashedPassword,
        DateTime dateRegistered,
        string authToken)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        HashedPassword = hashedPassword;
        DateRegistered = dateRegistered;
        AuthToken = authToken;
    }

    public User() { }
}
