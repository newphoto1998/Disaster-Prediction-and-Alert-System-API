using System;
using System.Collections.Generic;

namespace DisasterPredictionAPI.Models;

public partial class DisasterLog
{
    public int LogId { get; set; }

    public string? LogRegion { get; set; }

    public string? LogDisasterType { get; set; }

    public int? LogRiskScore { get; set; }

    public string? LogRiskLevel { get; set; }

    public decimal? LogLatitude { get; set; }

    public decimal? LogLongitude { get; set; }

    public int? Rev { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }
}
