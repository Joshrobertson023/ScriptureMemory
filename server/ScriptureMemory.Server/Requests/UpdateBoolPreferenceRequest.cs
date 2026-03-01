namespace VerseAppNew.Server.Requests;

public class UpdateBoolPreferenceRequest
{
    public int UserId { get; set; }
    public bool Enabled { get; set; }
}
