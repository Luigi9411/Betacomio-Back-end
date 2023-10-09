using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Betacomio.Models;

public partial class NewCustomer
{
    public int CustomerId { get; set; }

    [EmailAddress]
    [Required]
    public string EmailAddress { get; set; } = null!;
    [MinLength(4)]
    [Required]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&_-])[a-zA-Z0-9!@#$%^&_-]{8,}$", ErrorMessage = "Invalid Password")]
    public string PasswordHash { get; set; } = null!;

    public string? PasswordSalt { get; set; }

    public string? Role { get; set; }
}
