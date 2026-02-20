using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CopyVerses
    {
        private readonly string oracleConnectionString;
        private readonly string sqlServerConnectionString;

        public CopyVerses(string oracleConnectionString, string sqlServerConnectionString)
        {
            this.oracleConnectionString = oracleConnectionString;
            this.sqlServerConnectionString = sqlServerConnectionString;
        }

        public async Task SyncVersesAsync()
        {
            using var oracleConnection = new OracleConnection(oracleConnectionString);
            using var sqlConnection = new SqlConnection(sqlServerConnectionString);

            await oracleConnection.OpenAsync();
            await sqlConnection.OpenAsync();

            var selectCmd = new OracleCommand("SELECT VerseId, Reference, Text FROM Verses", oracleConnection);
            using var reader = await selectCmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var verseId = reader.GetInt32(0);
                var reference = reader.GetString(1);
                var text = reader.GetString(2);

                var insertCmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM Verses WHERE Reference = @Reference)
                    INSERT INTO Verses (VerseId, Reference, Text)
                    VALUES (@VerseId, @Reference, @Text)", sqlConnection);

                insertCmd.Parameters.AddWithValue("@VerseId", verseId);
                insertCmd.Parameters.AddWithValue("@Reference", reference);
                insertCmd.Parameters.AddWithValue("@Text", text);

                await insertCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine("✅ Verse data successfully synced from Oracle to SQL Server.");
        }
    }
}
