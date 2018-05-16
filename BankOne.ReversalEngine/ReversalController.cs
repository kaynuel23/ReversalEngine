using BankOne.ReversalEngine.Data;
using BankOne.ReversalEngine.Model;
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
        GenericUnitOfWork uow = null;
        ReversalsRepository reversalDAO = null;
        private BankOneReversal.Tracing.Logger logger;
        public ReversalController()
        {
            uow = new GenericUnitOfWork("Reversals");
            reversalDAO = new ReversalsRepository("Reversals");
            logger = new BankOneReversal.Tracing.Logger();
        }
        [Route("DoTransactionReversalByUniqueIdentifier")]
        [HttpPost]
        public async Task<bool> DoTransactionReversal(string mfbCode, string uniqueIdentifier)
        {
            //Here we log this and return true
            try
            {
                //check if it has been sent before
                //return true if so
                logger.Log($"Log {uniqueIdentifier} for {mfbCode}");
                int reversalCount = await reversalDAO.ReversalExists(mfbCode, uniqueIdentifier);
                    //uow.Repository<Reversals>().GetAsync(filter: x => x.UniqueIdentifier == uniqueIdentifier && x.MFBCode == mfbCode);
                if (reversalCount > 0)
                {
                    logger.Log($"Exists {uniqueIdentifier} for {mfbCode}");
                    return true;
                }
                //int result = await uow.Repository<Reversals>().InsertAsync(new Reversals() { UniqueIdentifier = uniqueIdentifier, MFBCode = mfbCode, DateLogged = DateTime.Now });
                await reversalDAO.Insert(mfbCode, uniqueIdentifier);
                logger.Log($"Done Logging {uniqueIdentifier} for {mfbCode}");
                return true;
            }
            catch (Exception ex)
            {
                //log the error
                logger.Log($"Error {uniqueIdentifier} for {mfbCode} - {ex.Message}. {ex.StackTrace}");
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
