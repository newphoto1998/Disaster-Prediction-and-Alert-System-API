using System;
using System.Collections.Generic;

namespace DisasterPredictionAPI.Models;

public partial class AlertSetting
{
    public string RegionId { get; set; } = null!;

    public string? AlertDisasterTypes { get; set; } = null!;

    public int AlertThresholdScore { get; set; }
}
