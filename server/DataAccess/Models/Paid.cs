using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;

public sealed class Paid
{
    public bool IsPaymentActive { get; set; } = false;
    public DateTime? DatePaid { get; set; }
    public DateTime? DateExpired { get; set; }
}
