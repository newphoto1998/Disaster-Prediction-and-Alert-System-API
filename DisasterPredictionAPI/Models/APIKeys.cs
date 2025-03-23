namespace DisasterPredictionAPI.Models
{
    public class KeyAPIs
    {

        public string? SECRET_KEY_WEATHER { get; set; }

        public string? SECRET_KEY_LINE { get; set; }

        public string? SECRET_KEY_LINE_CHANEL { get; set; }


        public KeyAPIs(string weather , string secretLine , string chanelLine)
        {
            SECRET_KEY_WEATHER = weather;
            SECRET_KEY_LINE = secretLine;
            SECRET_KEY_LINE_CHANEL = chanelLine;
        }
    }
}
