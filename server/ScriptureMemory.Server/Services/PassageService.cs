using DataAccess.DataInterfaces;

namespace ScriptureMemory.Server.Services;

public interface IPassageService
{

}

public class PassageService : IPassageService
{
    private readonly IUserPassageData passageContext;

    public PassageService(IUserPassageData passageContext)
    {
        this.passageContext = passageContext;
    }


}
