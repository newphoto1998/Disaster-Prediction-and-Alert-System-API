using System;
using System.Collections.Generic;

namespace DisasterPredictionAPI.Models;

public partial class Region
{
    public string RegionId { get; set; } = null!;

    public string RegionDisasterTypes { get; set; } = null!;

    public decimal? RegionLatitude { get; set; }

    public decimal? RegionLongitude { get; set; }
}
