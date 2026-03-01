using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data;

public sealed class PaidData : IPaidData
{
    private readonly IDbConnection conn;

    public PaidData(IDbConnection connection)
    {
        conn = connection;
    }

    public async Task CreatePaidData(Paid paid, string username)
    {

    }
}
