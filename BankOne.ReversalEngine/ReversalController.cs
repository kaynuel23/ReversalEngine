using BankOne.ReversalEngine.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BankOne.ReversalEngine
{
    public class TempClassForPublish
    {
        public string TransactionTime { get; set; }
        public string Institution { get; set; }
        public string UniqueId { get; set; }
        public string IsResponse { get; set; }
        public string PostingResponse { get; set; }
        public string PostingType { get; set; }
        public string UniqueLogID { get; set; }
        public string PostingsUrlID { get; set; }
    }
    [RoutePrefix("api/Reversal")]
    public class ReversalController : ApiController
    {
        [Route("DoTransactionReversalByUniqueIdentifier")]
        [HttpPost]
        public async Task<bool> DoTransactionReversal(string mfbCode, string uniqueIdentifier)
        {
            //Here we log this and return true
            try
            {
                int result = await new ReversalsRepository("Reversals").Insert(mfbCode, uniqueIdentifier);
                return true;
            }
            catch (Exception)
            {
                //log the error
                return false;
            }
        }
        
        private static string ConvertToCommaSeparated(TempClassForPublish publishInfo, string stage, string errorCode)
        {
            return string.Join(",", new string[] {  publishInfo.Institution,  publishInfo.UniqueId,  DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fffffff tt"),
               publishInfo.IsResponse, publishInfo.PostingResponse, publishInfo.PostingType, publishInfo.UniqueLogID, stage,errorCode, publishInfo.PostingsUrlID});
        }
        public bool IsConnected()
        {
            return true;
        }
    }
}
