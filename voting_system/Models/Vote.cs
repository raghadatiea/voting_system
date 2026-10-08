using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class Vote
{
    public long VoteId { get; set; }

    public long ExhibitorId { get; set; }

    public long VisitorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Exhibtior Exhibitor { get; set; } = null!;

    public virtual Visitor Visitor { get; set; } = null!;
}
