using BankOneReversal;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BankOne.ReversalEngine
{
    public partial class ReversalEngine : ServiceBase
    {
        private IDisposable _server = null;
        private BankOneReversal.Tracing.Logger logger;
        private const string ASTERIKS = "****************************************************************";
        private const string BLANK_LINE = "                                                               ";
        private Engine reversalService = null;
        public ReversalEngine()
        {
            InitializeComponent();
            logger = new BankOneReversal.Tracing.Logger();
            reversalService = new Engine();
        }
        protected override void OnStart(string[] args)
        {
            logger.Log(ASTERIKS);
            logger.Log("Starting BankOne.ReversalEngine...");
            _server = WebApp.Start<Startup>(url: System.Configuration.ConfigurationManager.AppSettings["BankOne.ReversalEngine.BaseAddress"]);
            logger.Log("Loaded BankOne.ReversalEngine Web Api...");
            
            logger.Log("Started BankOne.ReversalEngine timer...");
            //Initialize the section that completes ReversalEngine to be posted.
            reversalService.Start();
            logger.Log("Started BankOne.ReversalEngine.");
            logger.Log(ASTERIKS);
        }

        protected override void OnStop()
        {
            logger.Log(ASTERIKS);
            logger.Log("Stopping BankOne.ReversalEngine...");
            if (_server != null)
            {
                _server.Dispose();
            }
            base.OnStop();
            try
            {
                reversalService.Stop();
            }
            catch { }
            logger.Log("Stopped BankOne.ReversalEngine.");
            logger.Log(ASTERIKS);
            logger.Log(BLANK_LINE);
        }
    }
}