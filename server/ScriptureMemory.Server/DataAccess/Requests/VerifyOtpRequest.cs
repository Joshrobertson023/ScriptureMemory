using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;

public sealed class VerifyOtpRequest
{
    [Required] public int UserId { get; set; }
    [Required] public string Otp { get; set; } = string.Empty;
}
