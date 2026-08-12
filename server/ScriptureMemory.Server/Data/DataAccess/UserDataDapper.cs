using Npgsql;

namespace DataAccess.Data;

/// <summary>
/// Raw-SQL user queries that don't fit EF Core LINQ. Not IUserData -- EF Core owns that
/// interface (see UserDataEFCore); this class is injected directly wherever raw/complex
/// queries are needed.
///
/// This is currently a minimal shell: the previous contents targeted an Oracle database with
/// a schema that no longer matches ApplicationDbContext at all, so they were retired rather
/// than translated. Migrate methods back in here (against the current Postgres schema) as
/// they're actually needed -- the simple single-column reads/updates belong in
/// UserDataEFCore instead.
/// </summary>
public class UserDataDapper
{
    private readonly NpgsqlDataSource _dataSource;

    public UserDataDapper(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
}
