using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankOneReversal
{
    public static class PostingsMiddleware
    {
        public static string url = System.Configuration.ConfigurationManager.AppSettings["CacheAPIUrl"];
        public static string mfbUrl = System.Configuration.ConfigurationManager.AppSettings["MfbAPIUrl"];
        public static string GetPostingsURL(string mfbCode)
        {
            string result = "";
            try
            {
                //http://10.1.2.14:9020/apis/bankone/institutionPostingUrlService/GetPostingUrlByInstitutionCode?institutionCode=0
                string uri = $@"{url}/institutionPostingUrlService/GetPostingUrlByInstitutionCode?institutionCode={mfbCode}";
                WebRequest request = WebRequest.Create(uri);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(uri);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (WebResponse response = request.GetResponse())
                {
                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(responseFromServer);
                }
            }
            catch (Exception ex)
            {
                return "";
            }

            return result;
        }
        public static Mfb GetByInstitutionCode(string mfbCode)
        {
            Mfb result = null;
            try
            {
                //http://10.1.2.13:9020/apis/bankone/mfbService/GetMfbByInstitutionCode?institutionCode=100040
                string uri = $@"{mfbUrl}/mfbService/GetMfbByInstitutionCode?institutionCode={mfbCode}";
                WebRequest request = WebRequest.Create(uri);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(uri);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                using (WebResponse response = request.GetResponse())
                {
                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<Mfb>(responseFromServer);
                }
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
    }
}
