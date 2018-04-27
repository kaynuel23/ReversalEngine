using BankOne.ReversalEngine.Data;
using BankOne.ReversalEngine.Model;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOneReversal
{
    public class Engine
    {
        private System.Timers.Timer timer = null;
        private BankOneReversal.Tracing.Logger logger;
        private GenericUnitOfWork guow = null;
        public Engine()
        {
            this.timer = new System.Timers.Timer();
            this.timer.AutoReset = false;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            logger = new Tracing.Logger();
            guow = new GenericUnitOfWork("Reversals");
        }

        public void Start()
        {
            //this.timer.Enabled = true;
            this.timer.Interval = 10000; //10 seconds
            this.timer.Start();
        }


        public void Stop()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
            }
        }


        async void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.timer.Stop();
            try
            {
                logger.Log($"Begin Reversals Processing");
                IEnumerable<Reversals> results = await guow.Repository<Reversals>().GetTopNAsync(x => x.ReversalStatus == ReversalStatus.Pending, null, "", 20);
                //new ReversalsRepository("Reversals").GetPendingReversals();
                logger.Log($"Reversals to process {results.Count()}");
                await new ReversalsRepository("Reversals").BulkUpdate(results.ToList());
                Parallel.ForEach(results, async reversal =>
                {
                    logger.Log($"Process");
                    bool result = await Policy
                        .Handle<Exception>()
                        //.CircuitBreakerAsync(7, TimeSpan.FromSeconds(40)
                        .WaitAndRetryForeverAsync(
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        async (exception, timespan) =>
                        {
                            //log error here and to grafana
                            logger.Log($"An Error occured: {exception.Message} - {exception.StackTrace}");
                            reversal.RetryCount += 1;
                            reversal.ReversalStatus = ReversalStatus.Processing;
                            await guow.Repository<Reversals>().UpdateAsync(reversal);
                        })
                        .ExecuteAsync(async () =>
                               await ReversalService.ProcessReversal(reversal.MFBCode, reversal.UniqueIdentifier));
                    if (result)
                    {
                        logger.Log($"Done Processing");
                        //Update that it has been processed
                        reversal.ReversalStatus = ReversalStatus.Successful;
                        await guow.Repository<Reversals>().UpdateAsync(reversal);
                        //new ReversalsRepository("Reversals").Update(reversal);
                        logger.Log($"Done Updating");
                    }
                    //else
                    //{
                    //    logger.Log($"Wasnt successful");
                    //    //Update that it has been processed
                    //    reversal.RetryCount += 1;
                    //    reversal.ReversalStatus = ReversalStatus.Failed;
                    //    await guow.Repository<Reversals>().UpdateAsync(reversal);
                    //    //new ReversalsRepository("Reversals").Update(reversal);
                    //    logger.Log($"Done Updating");
                    //}
                });

                //this.timer.Interval = 100 * Convert.ToInt64(ConfigurationManager.AppSettings["BankOne.ReversalEngine.TimerInterval"]);
            }
            catch (Exception ex)
            {
                logger.Log($"Processing Error - {ex.Message}. {ex.StackTrace}");
            }
            finally
            {
                this.timer.Start();
            }
        }
    }
}
