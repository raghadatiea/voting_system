using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class Admin
{
    public long AdminId { get; set; }

    public string UserName { get; set; } = null!;

    public string? PasswordHashed { get; set; }
}
