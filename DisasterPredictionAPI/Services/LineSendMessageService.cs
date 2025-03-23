using System.Net.Http.Headers;
using System.Text;
using DisasterPredictionAPI.Models;
using Newtonsoft.Json;

namespace DisasterPredictionAPI.Services
{
    public class LineSendMessageService
    {

        public async Task<HttpResponseMessage> SendLineMessage(string secretKey, LineMessageAPI lineMessageAPIs)
        {

            HttpClient lineSendmessages = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            string json = JsonConvert.SerializeObject(lineMessageAPIs, Formatting.Indented);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");


            lineSendmessages.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
            HttpResponseMessage response = await lineSendmessages.PostAsync("https://api.line.me/v2/bot/message/push", content);


            return response;    
        }
    
    
    
        public async Task<List<TextDetail>> LineMessageTextDetail(List<DisasterRisksClass> _payload)
      
        {
            List<TextDetail> textList = new List<TextDetail>();

         
            
            foreach (DisasterRisksClass item in _payload)
            {
                string disasterTH = item.DisasterType.ToLower() == "flood" ? "น้ำท่วม" : (item.DisasterType.ToLower() == "wildfire" ? "ไฟป่า" : "แผ่นดินไหว");
                string levelTH = item.RiskLevel == "Low" ? "ต่ำ" : (item.RiskLevel == "Medium" ? "ปานกลาง" : "สูง");
                string unit = item.DisasterType.ToLower() == "flood" ? "mm/h" : (item.DisasterType.ToLower() == "wildfire" ? "%" : "Richter");

                TextDetail textDetail = new TextDetail();
                textDetail.type = "text";
                textDetail.text = "แจ้งเตือนภัยพิบัติ \n" +
                                   "ภูมิภาค : " + item.RegionID + "\n" +
                                    "ประเภท : " + disasterTH + "\n" +
                                    "ระดับความเสี่ยง : " + levelTH + "\n" +
                                    "รายละเอียด : ค่าที่วัดได้ " + item.RiskScore + " สูงกว่าเกณฑ์มาตราฐานที่กำหนด (Alert Threshold Score)\n" +
                                    "ตำแหน่ง : " + item.Latitude.ToString() + ", " + item.Longitude.ToString() + "\n" +
                                     "เวลา : " + Convert.ToDateTime(item.timeStamp).ToString("dd/MM/yyyy HH:mm");


                textList.Add(textDetail);
            }

          
            

            return textList;


        }
    
    }
}
