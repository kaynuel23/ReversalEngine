using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace BankOneReversal.Tracing
{
    public class Logger
    {
        public string UniqueKey { get; private set; }
        //PublisherAPI _publisher = null;
        public Logger()
        {
            UniqueKey = DateTime.Now.ToString("ddMMyyhhmmssfffffff");
            //_publisher = new PublisherAPI();
        }

        public Logger(string uniqueKey)
            :this()
        {
            UniqueKey = uniqueKey;
            //_publisher = new PublisherAPI();
        }
        
        public static string GetFullExceptionMessage(Exception ex)
        {
            string ex_msg = ex.Message;
            if (ex.InnerException != null)
            {
                while (ex.InnerException != null)
                {
                    var loaderEx = ex.InnerException as System.Reflection.ReflectionTypeLoadException;
                    if (loaderEx != null)
                    {
                        var loaderInnerExes = loaderEx.LoaderExceptions;
                        for (int i = 0; i < loaderInnerExes.Length; i++)
                        {
                            ex_msg += string.Format("Loader Exception {0}: {1}\n", (i + 1), loaderInnerExes[i].Message);
                        }
                    }
                    else
                    {
                        ex_msg += string.Format(" because {0} ", ex.InnerException.Message);
                    }
                    ex = ex.InnerException;
                }
            }
            else
            {
                var childEx = ex.InnerException as System.Reflection.ReflectionTypeLoadException;
                if (childEx != null)
                {
                    var loaderInnerExes = childEx.LoaderExceptions;
                    for (int i = 0; i < loaderInnerExes.Length; i++)
                    {
                        ex_msg += string.Format("Loader Exception {0}: {1}\n", (i + 1), loaderInnerExes[i].Message);
                    }
                }
                else // for other types of exceptions that may have other ways of reporting their details
                {

                }
            }
            return ex_msg;
        }

        public static void PrintReportLog(string description)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("[{0}] : {1}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss.fffffffK"), description));
        }
        
        private bool shouldLog;
        public void LogViaPostingType(string description, int pad)
        {
            //if (this.shouldLog) //bring it back later
            {
                //Log(description, pad);
                Trace.TraceInformation(string.Join("", new string[] { "{", UniqueKey, "} [", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss.fffffffK"), "] :", description }));
            }
        }
        
        public void Log(string description, int pad = 1)
        {
            Log(description, pad, "");
            //_publisher.APIMethod(description);
        }

        public void Log(string description, int pad, string namePrefix)
        {
            Trace.TraceInformation(string.Join("", new string[] { "{", UniqueKey, "} [", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss.fffffffK"), "] :", description }));
            //Trace.TraceInformation(string.Join("", new string[] { "[", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss.fffffffK"), "] :", description }));
        }


        private static readonly object lockObject = new object();
    }
}
