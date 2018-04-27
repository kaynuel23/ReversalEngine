using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankOneReversal
{
    public class ReversalService
    {
        private static string postingsUrl = System.Configuration.ConfigurationManager.AppSettings["ISO8583APIUrl"];// "http://localhost:9050/api/Iso8583/";//config
        public static async Task<bool> ProcessReversal(string institutionCode, string originalDataElements)
        {
            try
            {
                string uniqueIdentifier = Guid.NewGuid().ToString();
                string publishInfoMessage = null;
                PublisherAPI redisAPI = new PublisherAPI(ConfigurationManager.AppSettings["ISOChannelName"]);
                TempClassForPublish publishInfo = null;
                publishInfo = new TempClassForPublish
                {
                    Institution = institutionCode,
                    UniqueId = uniqueIdentifier,
                    IsResponse = "false",
                    PostingResponse = "About to Post",
                    PostingType = "6"
                };
                publishInfoMessage = TempClassForPublish.ConvertToCommaSeparated(publishInfo, "1", "");
                redisAPI.APIMethod(PublisherAPI.ConvertObjectToString(publishInfoMessage));
                string postingsMiddlewareUrl = PostingsMiddleware.GetPostingsURL(institutionCode);
                if (!string.IsNullOrEmpty(postingsMiddlewareUrl))
                {
                    System.Diagnostics.Trace.TraceInformation($"Redirect URI - {postingsMiddlewareUrl}");
                    try
                    {
                        postingsUrl = $"{postingsMiddlewareUrl.Split(new string[] { "api" }, StringSplitOptions.RemoveEmptyEntries)[0]}api/Iso8583/";
                        System.Diagnostics.Trace.TraceInformation($"Redirect URI - {postingsUrl}");
                    }
                    catch
                    {
                    }
                }
                else { System.Diagnostics.Trace.TraceInformation($"Default URI - {postingsUrl}"); }
                string uri = $"{postingsUrl}DoTransactionReversalByUniqueIdentifier?mfbCode={institutionCode}&uniqueIdentifier={originalDataElements}";
                System.Diagnostics.Trace.TraceError($"URI is {uri}");
                using (var request = new WebClient())
                {
                    request.Headers[HttpRequestHeader.Accept] = "application/json";
                    request.Headers[HttpRequestHeader.ContentType] = "application/json";
                    //request.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
                    string response = await request.UploadStringTaskAsync(new Uri(uri), "POST", "");
                    return JsonConvert.DeserializeObject<bool>(response);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(String.Format("{0} : {1}", ex.Message, ex.StackTrace));
                throw ex;
                //return false;
            }
        }
    }
}
