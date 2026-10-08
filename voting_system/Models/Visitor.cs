using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class Visitor
{
    public long VisitorId { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsPhoneVerified { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
