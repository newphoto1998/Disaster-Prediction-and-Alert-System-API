using DisasterPredictionAPI.Models;
using Newtonsoft.Json.Linq;

namespace DisasterPredictionAPI.Services
{
    public class CalculateScroceService
    {   

        WeatherAPIService APIService = new WeatherAPIService();

        public async Task<double> calScoreRisk(string? API_KEY, string Type, decimal? lat, decimal? lon)
        {
            string disasterType = Type.ToLower();
            double measuredVal = 0.00;

            if (disasterType == "flood" || disasterType == "wildfire")
            {
                measuredVal = await APIService.WeatherAPI(API_KEY, disasterType, lat, lon);
                return measuredVal;
            }
            
            else if (disasterType == "earthquake") {

                measuredVal = await APIService.USGSA(lat, lon);
                return measuredVal;
            }
            else
            {
                throw new Exception("ไม่พบข้อมูล");

            }


        }




        public async Task<string> calLevelRisk(double Measurement, int riskScore, string disasterType)
        {
            string level = "";
            double? low = riskScore / 3.0;
            double? mid = (2.0 / 3.0) * (double)riskScore;

            if (Measurement == 0) {
                level = disasterType == "flood" ? "ไม่สามารถวิเคราะห์ค่าได้ เนื่องจากไม่พบฝนในพื้นที่" : "ไม่สามารถวิเคราะห์ค่าได้ เนื่องจากไม่พบข้อมูลแผ่นดินไหวในระยะที่กำหนด";
            }

            else if (Measurement >= 0 && Measurement < low)
            {
                level = "Low";
            }
            else if (Measurement >= low && Measurement < mid)
            {
                level = "Medium";
            }
            else if (Measurement >= mid || Measurement >= riskScore)
            {
                level = "High";
            }
           

               return level;
        }



    }
}
