using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;

public sealed class ResetPasswordRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Otp { get; set; } = string.Empty;
    [Required] public string NewPassword { get; set; } = string.Empty;
}
