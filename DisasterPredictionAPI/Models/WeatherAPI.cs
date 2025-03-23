namespace DisasterPredictionAPI.Models
{
    public class WeatherAPI
    {
     

        public class Flood
        {

            public double mm_h { get; set; }

        }


        public class Wildfire
        {

            public double temperature { get; set; }
            public double humidity { get; set; }


            public Wildfire(double temp , double humi)
            {
                temperature = temp;
                humidity = humi;
            }


            public double ConvertTempTocel(double temp)
            {
                return temp - 273.15;
            }


        }



    }
}
