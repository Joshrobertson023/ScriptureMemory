using Bogus;
using DataAccess.Models;

namespace VerseAppNew.Server.Bogus;

public sealed class UserGenerator : Faker<User>
{
    public UserGenerator()
    {
        RuleFor(u => u.Username, f => f.Internet.UserName());
        RuleFor(u => u.FirstName, f => f.Name.FirstName());
        RuleFor(u => u.LastName, f => f.Name.LastName());
        RuleFor(u => u.Email, f => f.Internet.Email());
        RuleFor(u => u.HashedPassword, f => f.Internet.Password());
    }
}

public sealed class SearchGenerator : Faker<Search>
{
    public SearchGenerator()
    {
        RuleFor(s => s.SearchTerm, f => f.Lorem.Word());
        RuleFor(s => s.SearchType, f => f.PickRandom<VerseAppLibrary.Enums.SearchType>());
    }
}
