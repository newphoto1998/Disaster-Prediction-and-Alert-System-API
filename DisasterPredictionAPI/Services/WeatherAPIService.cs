namespace DisasterPredictionAPI.Services;

using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DisasterPredictionAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class WeatherAPIService
{



    private const string KeyVaultUrl = "https://lsc-azurekv-demo.vault.azure.net/";
    string baseURL_WEATHER_API = "https://api.openweathermap.org/";
    string baseURL_USGSA_API = "https://earthquake.usgs.gov/";



    public async Task<double> WeatherAPI(string? api_key,string type, decimal? latitude, decimal? longitud)
    {
        HttpClient _http = new HttpClient();

        _http.BaseAddress = new Uri(baseURL_WEATHER_API);

        var respone_weatherAPI = await _http.GetAsync($@"data/2.5/weather?lat={latitude}&lon={longitud}&appid={api_key}");
        if (respone_weatherAPI.IsSuccessStatusCode)
        {
            var result = await respone_weatherAPI.Content.ReadAsStringAsync();
            dynamic? json = (JObject?)JsonConvert.DeserializeObject(result);
            string weatherStatus = json.weather[0].main;


            // ถ้ามีพื้นที่บริเวณนั้นมีฝนตก และ สถานะของ alert setting = flood
            if (weatherStatus == "Rain" && type.ToLower() == "flood")
            {
                // ดึงค่า ปริมาณฝน mm/h
                return json["rain"].Value<decimal>("1h");

            }
            else if (type == "wildfire")
            {
                decimal temp = json["main"].Value<int>("temp");
                decimal humidity = json["main"].Value<int>("humidity");

                //  high temperatures with low humidity increase

                temp = temp - (decimal)273.15;

                return (double)(100 - humidity + temp);

            }



        }

        return 0;


    }



    public async Task<double> USGSA(decimal? latitude, decimal? longitud)
    {
        HttpClient _http = new HttpClient();

        _http.BaseAddress = new Uri(baseURL_USGSA_API);
        var respone_earthQuakeAPI = await _http.GetAsync($@"fdsnws/event/1/query?format=geojson&latitude={latitude}&longitude={longitud}&maxradius=100");
        if (respone_earthQuakeAPI.IsSuccessStatusCode)
        {
            var result_earthQuake = await respone_earthQuakeAPI.Content.ReadAsStringAsync();
            dynamic? jsonEarthQuake = (JObject?)JsonConvert.DeserializeObject(result_earthQuake);

            if (jsonEarthQuake.metadata.count == 0)
            {
                return 0;
            }
            else
            {
                return jsonEarthQuake.features[0].properties.mag;

            }
        }


        return 0;




    }


 
}
