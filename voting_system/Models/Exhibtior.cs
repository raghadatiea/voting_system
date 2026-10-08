using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class Exhibtior
{
    public long ExhibitorsId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhotoUrl { get; set; } = null!;

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
