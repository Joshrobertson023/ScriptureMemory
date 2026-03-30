using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data;

public sealed class PaidData
{
    private readonly IDbConnection conn;

    public PaidData([FromKeyedServices("Postgres")] IDbConnection connection)
    {
        conn = connection;
    }

    public async Task CreatePaidData(Paid paid, string username)
    {

    }
}
