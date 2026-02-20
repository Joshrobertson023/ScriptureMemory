using Dapper;
using System.Data;

namespace DataAccess.Data;

public static class Convert
{
    public static int ToInt(bool value)
    {
        return value ? 1 : 0;
    }
}