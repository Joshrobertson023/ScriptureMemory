using Npgsql;

namespace ScriptureMemory.Server.DataAccess.Data;

public interface IRequestDbConnection
{
    NpgsqlConnection Connection { get; }
}

public sealed class RequestDbConnection : IRequestDbConnection, IDisposable
{
    private readonly NpgsqlConnection _connection;

    public RequestDbConnection(NpgsqlDataSource dataSource)
    {
        _connection = dataSource.OpenConnection();
    }

    public NpgsqlConnection Connection => _connection;

    public void Dispose()
    {
        _connection.Dispose();
    }
}
