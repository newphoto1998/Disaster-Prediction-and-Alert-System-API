namespace DisasterPredictionAPI.Models
{
    public class RegionDisaster
    {

        public string RegionId { get; set; } = null!;

        public LocationCoordinatesModel LocationCoordinates { get; set; } = null!;

        public string[] DisasterTypes { get; set; } = null!;


      
    }

    public class LocationCoordinatesModel
    {
        public decimal? RegionLatitude { get; set; }

        public decimal? RegionLongitude { get; set; }
    }




}
