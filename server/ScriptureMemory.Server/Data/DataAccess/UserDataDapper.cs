using Npgsql;

namespace DataAccess.Data;

public class UserDataDapper
{
    private readonly NpgsqlDataSource _dataSource;

    public UserDataDapper(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
}
