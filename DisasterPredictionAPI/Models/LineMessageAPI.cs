namespace DisasterPredictionAPI.Models
{
    public class LineMessageAPI
    {
        public string? to { get; set; }
        public List<TextDetail>? messages { get; set; }
    }

    public class TextDetail
    {
        public string? type { get; set; } 

        public string? text { get; set; }
    }
}
