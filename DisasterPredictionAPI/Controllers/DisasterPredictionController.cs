using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure;
using DisasterPredictionAPI.Contexts;
using DisasterPredictionAPI.Models;
using DisasterPredictionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Core;
using System.Text.Json.Nodes;
using System.Text;
using System;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace DisasterPredictionAPI.Controllers
{

    public class DisasterPredictionController : ControllerBase
    {
        private readonly DBDEV efDEV;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _redisCache;
        WeatherAPIService srvWeather = new WeatherAPIService();
        CalculateScroceService srvCal = new CalculateScroceService();
        LineSendMessageService srvLineSendMessageService = new LineSendMessageService();
        private readonly KeyAPIs _secretKey;


        public DisasterPredictionController(DBDEV contextDBDEV , IHttpClientFactory httpClientFactory , IDistributedCache iDistributedCache , KeyAPIs keys)
        {
             efDEV = contextDBDEV;
            _httpClientFactory = httpClientFactory;
            _redisCache = iDistributedCache;
            _secretKey = keys;

        }

      

        [HttpPost]
        [Route("/api/regions")]
        public  IActionResult AddRegion([FromBody] RegionDisaster regionDisaster)
        {
      
     

            List<Region> checkDuplicate = efDEV.Regions.Where(x => x.RegionId == regionDisaster.RegionId
                                                    && regionDisaster.DisasterTypes.Contains(x.RegionDisasterTypes)).ToList();

            if (checkDuplicate.Count > 0)
            {
                string message = "";
                foreach (Region region in checkDuplicate) {
                    message += $"Region: {region.RegionId} and Disaster : {region.RegionDisasterTypes} is duplicate ,";
                }

                message = message.Substring(0, message.Length-1);


                return BadRequest(new { status="Error",message = $"{message}" });
            }

            foreach (string item in regionDisaster.DisasterTypes)
            {   

              
                Region region = new Region();
                region.RegionId = regionDisaster.RegionId;
                region.RegionLatitude = regionDisaster.LocationCoordinates.RegionLatitude;
                region.RegionLongitude = regionDisaster.LocationCoordinates.RegionLongitude;
                region.RegionDisasterTypes = item;
            

         

                efDEV.Regions.Add(region);  
                efDEV.SaveChanges();
            }





            return Ok(new { 
                status = "success",
                message = $"Region is created"
                
            
            });
        }


        [HttpPost]
        [Route("/api/alert-settings")]
        public IActionResult ConfigureAlertSetting([FromBody] AlertSetting _alert)
        {

            var checkRegion = efDEV.Regions.Where(x => x.RegionId == _alert.RegionId && x.RegionDisasterTypes == _alert.AlertDisasterTypes).ToList();

            if (checkRegion.Count > 0)
            {
                var checkDuplicate = efDEV.AlertSettings.Where(y => y.RegionId == _alert.RegionId && y.AlertDisasterTypes == _alert.AlertDisasterTypes).FirstOrDefault();
                
                if (checkDuplicate != null) {
                    return NotFound(new { status = "Error", message = "Region or Disaster is duplicate data" });
                }


                efDEV.AlertSettings.Add(_alert);
                efDEV.SaveChanges();
                return Ok(new
                {
                    status = "success",
                    message = "alert settings have been saved."


                });


            }
            else
            {
                return NotFound(new {status = "Error" , message = "Region or Disaster not register data"});
            }



        }


     


        [HttpGet]
        [Route("/api/disaster-risks")]
        public async Task<IActionResult> DisasterRisks()
        {



           string? keySecret = _secretKey.SECRET_KEY_LINE;
           string? keyLineChanel = _secretKey.SECRET_KEY_LINE_CHANEL;

            var jsonDisasterRiskData = await _redisCache.GetStringAsync("jsondata");


            if (string.IsNullOrEmpty(jsonDisasterRiskData))
            {


                List<DisasterRisksClass> disasterRisksClasses = new List<DisasterRisksClass>();

   
                var Alerts = (from objAlert in efDEV.AlertSettings
                              join objRegion in efDEV.Regions
                              on new { region = objAlert.RegionId, type = objAlert.AlertDisasterTypes }
                              equals new { region = objRegion.RegionId, type = objRegion.RegionDisasterTypes} into isJoin
                              from newItem in isJoin.DefaultIfEmpty()
                              select new { objAlert.RegionId, objAlert.AlertDisasterTypes,objAlert.AlertThresholdScore, newItem.RegionLatitude, newItem.RegionLongitude }).ToList();

                foreach (var item in Alerts)
                {


                    DisasterRisksClass disasterRisksClass = new DisasterRisksClass();

                    string regionID = item.RegionId.Trim();
                    string? disasterTypes = item.AlertDisasterTypes;
                    decimal? latitude = item.RegionLatitude;
                    decimal? longitude = item.RegionLongitude;

                    disasterRisksClass.RegionID = regionID;
                    disasterRisksClass.DisasterType = item.AlertDisasterTypes;
                    disasterRisksClass.RiskScore = await srvCal.calScoreRisk(_secretKey.SECRET_KEY_WEATHER, item.AlertDisasterTypes, latitude, longitude);
                    disasterRisksClass.RiskLevel = await srvCal.calLevelRisk(disasterRisksClass.RiskScore, item.AlertThresholdScore, disasterTypes);
                    disasterRisksClass.AlertTriggred = disasterRisksClass.RiskLevel == "High" ? true : false;

                    disasterRisksClass.Latitude = latitude;
                    disasterRisksClass.Longitude = longitude;
                    disasterRisksClass.timeStamp = DateTime.Now;

                    disasterRisksClasses.Add(disasterRisksClass);


                    if ((bool)disasterRisksClass.AlertTriggred)
                    {
                        List<DisasterRisksClass> currentDtLoop = disasterRisksClasses.Where(x => x.AlertTriggred == true && x.RegionID == regionID && x.DisasterType == disasterTypes).ToList();
                        LineMessageAPI lineMessageHeaderDtLoop = new LineMessageAPI();

                        lineMessageHeaderDtLoop.to = keyLineChanel;
                        lineMessageHeaderDtLoop.messages = await srvLineSendMessageService.LineMessageTextDetail(currentDtLoop);

                        HttpResponseMessage responseMessagedtLoop = await srvLineSendMessageService.SendLineMessage(keySecret, lineMessageHeaderDtLoop);

                        if (responseMessagedtLoop.IsSuccessStatusCode)
                        {



                        }
                        else
                        {
                            return BadRequest(new { status = "Error", message = "Failed external API calls." });
                        }
                    }
                


                }

                jsonDisasterRiskData = JsonConvert.SerializeObject(disasterRisksClasses);

                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(30));
                _redisCache.SetString("jsondata", jsonDisasterRiskData, options);



                // บันทึกลง log
                if (disasterRisksClasses.Count > 0)
                {

                    List<DisasterLog> listPrev = efDEV.DisasterLogs.ToList();
                    if (listPrev.Count > 0)
                    {
                        listPrev.ForEach(a => a.Rev = 1);
                        efDEV.SaveChanges();
                    }

                    foreach (var item in disasterRisksClasses)
                    {
                        DisasterLog log = new DisasterLog();


                        log.CreateBy = "SYSTEM";
                        log.CreateDate = DateTime.Now;
                        log.LogDisasterType = item.DisasterType;
                        log.LogRiskScore = (int?)item.RiskScore;
                        log.LogRiskLevel = item.RiskLevel;
                        log.LogRegion = item.RegionID;
                        log.LogLatitude = (decimal?)item.Latitude;
                        log.LogLongitude = (decimal?)item.Longitude;


                        log.Rev = 999;

                        efDEV.DisasterLogs.Add(log);
                        efDEV.SaveChanges();

                    }


                }
     
                return Ok(new { status = "success", data = disasterRisksClasses.Select(i => new { i.RegionID, i.DisasterType, i.RiskScore, i.RiskLevel, i.AlertTriggred }) });

            }
            else
            {
                List<DisasterRisksClass> disasterRisksResult = JsonConvert.DeserializeObject<List<DisasterRisksClass>>(jsonDisasterRiskData);
                List<DisasterRisksClass> AlertData = disasterRisksResult.Where(x => x.AlertTriggred == true).ToList();

                LineMessageAPI lineMessageHeader = new LineMessageAPI();
                lineMessageHeader.to = keyLineChanel;
                lineMessageHeader.messages = await srvLineSendMessageService.LineMessageTextDetail(AlertData);

                HttpResponseMessage responseMessage = await srvLineSendMessageService.SendLineMessage(keySecret, lineMessageHeader);

                if (responseMessage.IsSuccessStatusCode)
                {

                    return Ok(new { status = "sucess", data = disasterRisksResult.Select(i => new { i.RegionID, i.DisasterType, i.RiskScore, i.RiskLevel, i.AlertTriggred }) });

                }
                else
                {
                    return BadRequest(new { status = "Error", message = "Failed external API calls." });
                }
            }


        }










        [HttpPost]
        [Route("/api/alerts/send")]
        public async Task<IActionResult> AlertsSend()
        {



            string? keySecret = _secretKey.SECRET_KEY_LINE;
            string? keyLineChanel = _secretKey.SECRET_KEY_LINE_CHANEL;


            var jsonDisasterRiskData = await _redisCache.GetStringAsync("jsondata");


            if (string.IsNullOrEmpty(jsonDisasterRiskData))
            {


                var logList = efDEV.DisasterLogs.Where(x => x.Rev == 999 && x.LogRiskLevel == "High").ToList();

                if (logList.Count <= 0)
                {
                    return NotFound(new { Status = "Error", message = "data not found" });
                }
                
                List<DisasterRisksClass> disasterRisksList = new List<DisasterRisksClass>();
                foreach(DisasterLog log in logList)
                {
                    DisasterRisksClass disasterRisksClass = new DisasterRisksClass();
                    disasterRisksClass.RegionID = log.LogRegion;
                    disasterRisksClass.DisasterType = log.LogDisasterType;
                    disasterRisksClass.RiskScore = Convert.ToDouble(log.LogRiskScore);
                    disasterRisksClass.RiskLevel = log.LogRiskLevel;
                    disasterRisksClass.AlertTriggred = log.LogRiskLevel == "High" ? true : false;

                    disasterRisksClass.Latitude = log.LogLatitude;
                    disasterRisksClass.Longitude = log.LogLongitude;
                    disasterRisksClass.timeStamp = log.CreateDate;

                    disasterRisksList.Add(disasterRisksClass);
                }

                List<DisasterRisksClass> currentAlertData = disasterRisksList.Where(x => x.AlertTriggred == true).ToList();


                LineMessageAPI lineMessageHeader = new LineMessageAPI();
                lineMessageHeader.to = keyLineChanel;
                lineMessageHeader.messages = await srvLineSendMessageService.LineMessageTextDetail(currentAlertData);


                HttpResponseMessage responseMessage = await srvLineSendMessageService.SendLineMessage(keySecret, lineMessageHeader);



                if (responseMessage.IsSuccessStatusCode)
                {

                    return Ok(new { status = "success", message = "data is send." });

                }
                else
                {
                    return BadRequest(new { status = "Error" });
                }


            }
            else
            {


                List<DisasterRisksClass> disasterRisksResult = JsonConvert.DeserializeObject<List<DisasterRisksClass>>(jsonDisasterRiskData);

                List<DisasterRisksClass> currentAlertData = disasterRisksResult.Where(x => x.AlertTriggred == true).ToList();

                LineMessageAPI lineMessageHeader = new LineMessageAPI();
                lineMessageHeader.to = keyLineChanel;
                lineMessageHeader.messages = await srvLineSendMessageService.LineMessageTextDetail(currentAlertData); ;



                HttpResponseMessage responseMessage = await srvLineSendMessageService.SendLineMessage(keySecret, lineMessageHeader);



                if (responseMessage.IsSuccessStatusCode)
                {

                    return Ok(new { status = "success" , message = "data is send." });

                }
                else
                {
                    return BadRequest(new { status = "Error" });
                }
            }

            }




        [HttpGet]
        [Route("/api/alerts")]
        public async Task<IActionResult> Alerts()
        {

            var logList = from log in efDEV.DisasterLogs.Where(x => x.Rev == 999 && x.LogRiskLevel == "High")
                          select new { log.LogRegion, log.LogDisasterType, log.LogRiskLevel, 
                                        loaction = log.LogLatitude + ", " + log.LogLongitude , time = Convert.ToDateTime(log.CreateDate).ToString("dd/MM/yyyy HH:mm") };

            return Ok(logList);
        }



        //[HttpGet("get-secret")]
        //public async Task<IActionResult> GetSecret()
        //{
        //    try
        //    {
               

        //        return Ok(new { SecretName = _test });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = "Error accessing Key Vault", Error = ex.Message });
        //    }
        //}

    }

}
