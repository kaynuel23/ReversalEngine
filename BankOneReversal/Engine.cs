using BankOne.ReversalEngine.Data;
using BankOne.ReversalEngine.Model;
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

        public Engine()
        {
            this.timer = new System.Timers.Timer();
            this.timer.AutoReset = false;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
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
                Task<Reversals> results = await new ReversalsRepository("Reversals").GetPendingReversals();
                
                Parallel.ForEach(results, async reversal =>
                {
                    bool result = await ReversalService.ProcessReversal("1", "111");
                    if (result)
                    {
                        //Update that it has been processed
                    }
                });
                
                this.timer.Interval = 1000 * Convert.ToInt64(ConfigurationManager.AppSettings["ProcessInterval"]);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                this.timer.Start();
            }
        }
    }
}
