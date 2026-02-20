using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data;

public sealed class PaidData : IPaidData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public PaidData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task CreatePaidData(Paid paid, string username)
    {

    }
}
