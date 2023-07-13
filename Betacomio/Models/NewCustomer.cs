using System;
using System.Collections.Generic;

namespace Betacomio.Models;

public partial class NewCustomer
{
    public int CustomerId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PasswordSalt { get; set; }
}
