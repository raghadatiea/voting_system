using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class Category
{
    public long CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual ICollection<Exhibtior> Exhibitors { get; set; } = new List<Exhibtior>();
}
