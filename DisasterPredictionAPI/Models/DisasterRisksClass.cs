namespace DisasterPredictionAPI.Models
{
    public class DisasterRisksClass
    {

        public string? RegionID { get; set; }
        
        public string? DisasterType { get; set; }

        public double RiskScore { get; set; }

        public string? RiskLevel { get; set; }

        public Boolean? AlertTriggred { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public DateTime? timeStamp { get; set; }
    }
}
