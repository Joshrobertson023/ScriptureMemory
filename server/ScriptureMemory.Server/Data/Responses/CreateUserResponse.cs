namespace ScriptureMemory.Server.Data.Responses;

public class CreateUserResponse
{
    public User User { get; set; } = new();
    public string Jwt { get; set; } = string.Empty;
}