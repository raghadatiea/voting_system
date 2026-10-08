using System;
using System.Collections.Generic;

namespace voting_system.Models;

public partial class VotingSetting
{
    public long SettingId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double Radius { get; set; }
}
