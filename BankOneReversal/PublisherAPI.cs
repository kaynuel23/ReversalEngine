using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BankOneReversal
{
    public class TempClassForPublish
    {
        public string TransactionTime { get; set; }
        public string Institution { get; set; }
        public string UniqueId { get; set; }
        public string IsResponse { get; set; }
        public string PostingResponse { get; set; }
        public string PostingType { get; set; }
        public static string ConvertToCommaSeparated(TempClassForPublish publishInfo, string stage, string errorCode)
        {
            return string.Join(",", new string[] {  publishInfo.Institution,  publishInfo.UniqueId,  DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fffffff tt"),
               publishInfo.IsResponse, publishInfo.PostingResponse, publishInfo.PostingType, stage,errorCode});
        }
    }
    public class PublisherAPI
    {
        private static string publisherUrl = ConfigurationManager.AppSettings["PublisherUrl"];
        private static string _channelName = ConfigurationManager.AppSettings["ChannelName"];
        private static bool shouldPublishToRedis = Convert.ToBoolean(ConfigurationManager.AppSettings["shouldPublishToRedis"]);
        private static string institutionStatusUrl = ConfigurationManager.AppSettings["InstitutionStatusUrl"];
        public PublisherAPI()
        {

        }

        public PublisherAPI(string channelName)
        {
            _channelName = channelName;
        }

        public static string ConvertObjectToJson<T>(T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        }

        public static string ConvertObjectToString(string message)
        {
            return message;

        }

        public async void APIMethod(string message)
        {
            try
            {
                if (!shouldPublishToRedis && string.IsNullOrEmpty(publisherUrl) && string.IsNullOrEmpty(_channelName))
                {
                    return;
                }
                string uri = $"{publisherUrl}?channelName={_channelName}&Message={message}";
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    var content = new StringContent("");
                    await client.PostAsync(uri, content);
                }
                //WebRequest request = WebRequest.Create(uri);

                //request.Method = "POST";
                //byte[] byteArray = Encoding.UTF8.GetBytes(uri);
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = byteArray.Length;
                //Stream dataStream = request.GetRequestStream();

                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();
                //WebResponse response = request.GetResponse();
                //dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //string responseFromServer = reader.ReadToEnd();

            }
            catch
            {

            }
        }

        public static async Task<bool> GetInstitutionStatus(string mfbCode)
        {
            try
            {
                string uri = $"{institutionStatusUrl}?mfbcode={mfbCode}";
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    var content = new StringContent("");
                    HttpResponseMessage response = await client.PostAsync(uri, content);
                    Task<string> result = response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(result.ToString());
                }
                //WebRequest request = WebRequest.Create(uri);
                //request.Method = "POST";
                //byte[] byteArray = Encoding.UTF8.GetBytes(uri);
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = byteArray.Length;
                //Stream dataStream = request.GetRequestStream();

                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();
                //WebResponse response = request.GetResponse();
                //dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //string responseFromServer = reader.ReadToEnd();
                //result = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(responseFromServer);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
