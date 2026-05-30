using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;

/// <summary>
/// A user's payment information
/// </summary>
public sealed class PaidInfo
{
    [DefaultValue(false)]
    public bool IsPaymentActive { get; set; }
    
    public DateTime? DatePaid { get; set; }
    
    public DateTime? DateExpired { get; set; }

    public User UserNavigation { get; set; } = null!;
}
